using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Desktop.Core.Interfaces;
using ThePalace.Core.Desktop.Core.Models;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Desktop.Core
{
    public sealed class FormsManager : ApplicationContext, IDisposable
    {
        private bool IsDisposed = false;

        private static readonly Lazy<FormsManager> _current = new();
        public static FormsManager Current => _current.Value;

        private DisposableDictionary<string, FormBase> _forms = new();
        public IReadOnlyDictionary<string, FormBase> Forms => this._forms;

        public event EventHandler FormClosed = null;

        public FormsManager()
        {
            this.ThreadExit += new EventHandler((sender, e) => { ThreadManager.Current?.Dispose(); });
        }
        ~FormsManager() =>
            this.Dispose(false);

        public new void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            this.IsDisposed = true;

            FormClosed?.Clear();
            FormClosed = null;

            foreach (var form in _forms.Values)
            {
                var controls = form.Controls
                    .Cast<Control>()
                    .ToList();
                foreach (var control in controls)
                    try { control?.Dispose(); } catch { }
            }
            _forms?.Dispose();
            _forms = null;

            HotKeyManager.Current.Dispose();

            GC.SuppressFinalize(this);

            this.ExitThread();
        }

        private void _FormClosed(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            else if (sender is FormBase _sender &&
                UnregisterForm(_sender))
            {
                this.FormClosed?.Invoke(_sender, e);

                var forms = null as List<FormBase>;
                lock (this._forms)
                    forms = this._forms?.Values?.ToList() ?? new();
                var app = forms
                    .Where(f => f.GetType().Name == "Program")
                    .FirstOrDefault();
                if (!forms.Any(f =>
                    f != app &&
                    f is IFormDialog
                ))
                    new TCF(
                        ThreadManager.Current,
                        app,
                        this)
                    .Execute();
            }
        }

        //public static void InitializeUIUserRec(UserRec user)
        //{
        //    user.Extended.TryAdd(@"MessageQueue", new ConcurrentQueue<MsgBubble>());
        //    user.Extended.TryAdd(@"CurrentMessage", null);
        //}

        public bool RegisterForm(FormBase form, bool assignFormClosedHandler = true)
        {
            if (this.IsDisposed) return false;

            lock (this._forms)
                if (!this._forms.ContainsKey(form.Name))
                {
                    if (assignFormClosedHandler)
                    {
                        form.FormClosed += _FormClosed;
                        form.Disposed += _FormClosed;
                    }

                    this._forms.TryAdd(form.Name, form);

                    return true;
                }

            return false;
        }
        public bool UnregisterForm(FormBase form)
        {
            if (this.IsDisposed) return false;

            lock (this._forms)
                try
                {
                    var key = _forms
                        .Where(f => f.Value == form)
                        .Select(f => f.Key)
                        .FirstOrDefault();

                    if (key == null) return false;

                    _forms.TryRemove(key, out var _);

                    return true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

            return false;
        }
        public T CreateForm<T>(FormCfg cfg, bool assignFormClosedHandler = true)
            where T : FormBase, new()
        {
            if (this.IsDisposed) return null;

            var form = new T
            {
                Name = $"{typeof(FormBase).FullName}.{Guid.NewGuid()}",
                AutoScaleDimensions = cfg.AutoScaleDimensions,
                AutoScaleMode = cfg.AutoScaleMode,
                StartPosition = cfg.StartPosition,
            };

            if (cfg.Activated != null)
                form.Activated += cfg.Activated;
            if (cfg.Load != null)
                form.Load += cfg.Load;
            if (cfg.Shown != null)
                form.Shown += cfg.Shown;
            if (cfg.GotFocus != null)
                form.GotFocus += cfg.GotFocus;
            if (cfg.MouseMove != null)
                form.MouseMove += cfg.MouseMove;
            if (cfg.FormClosed != null)
                form.FormClosed += cfg.FormClosed;

            RegisterForm(form, assignFormClosedHandler);
            UpdateForm(form, cfg);

            return form;
        }
        public T GetForm<T>(string name = null)
            where T : FormBase
        {
            if (this.IsDisposed) return null;

            if (name == null) return null;
            else if (_forms.TryGetValue(name, out var value)) return value as T;
            else return null;
        }
        public static void UpdateForm<T>(T form, FormCfg cfg)
            where T : FormBase
        {
            form.SuspendLayout();

            form.ClientSize = cfg.Size;
            form.BackColor = cfg.BackColor;
            form.Padding = cfg.Padding;
            form.Margin = cfg.Margin;

            form.WindowState = cfg.WindowState;

            if (cfg.Text != null)
                form.Text = cfg.Text;

            form.ResumeLayout(false);
            form.PerformLayout();

            if (cfg.Visible) form.Show();
            else form.Hide();

            if (cfg.Focus) form.Focus();
        }

        public void RegisterControl(FormBase parent, Control control) =>
            parent.Controls.Add(control);
        public TControl[] CreateControl<TForm, TControl>(TForm parent, bool visible = true, params ControlCfg[] cfgs)
            where TForm : FormBase
            where TControl : Control, new()
        {
            if (this.IsDisposed) return null;

            if (parent == null ||
                (cfgs?.Length ?? 0) < 1)
                return null;

            var results = new List<TControl>();

            parent.SuspendLayout();

            foreach (var cfg in cfgs)
            {
                var control = new TControl();

                if (control is Button button)
                {
                    button.UseVisualStyleBackColor = cfg.UseVisualStyleBackColor;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = cfg.Value != 0;
                    checkBox.UseVisualStyleBackColor = cfg.UseVisualStyleBackColor;
                }
                else if (control is PictureBox pictureBox)
                {
                    pictureBox.BorderStyle = cfg.BorderStyle;
                }
                else if (control is ProgressBar progressBar)
                {
                    progressBar.Value = cfg.Value;
                }
                else if (control is RichTextBox richTextBox)
                {
                    richTextBox.MaxLength = cfg.MaxLength;
                    richTextBox.Multiline = cfg.Multiline;
                }
                else if (control is ScrollableControl scrollableControl &&
                    cfg.Scroll != null)
                {
                    scrollableControl.Scroll += cfg.Scroll;
                }
                else if (control is TextBox textBox)
                {
                    textBox.MaxLength = cfg.MaxLength;
                    textBox.Multiline = cfg.Multiline;
                }

                if (control != null)
                {
                    control.Name = $"{typeof(TControl).FullName}_{Guid.NewGuid()}";

                    if (cfg.Click != null)
                        control.Click += cfg.Click;

                    UpdateControl(control, cfg);

                    RegisterControl(parent, control);

                    results.Add(control);
                }
            }

            parent.Visible = visible;

            parent.ResumeLayout(false);
            parent.PerformLayout();

            return results.ToArray();
        }
        public static void UpdateControl(Control control, ControlCfg cfg)
        {
            control.Visible = cfg.Visible;
            control.TabIndex = cfg.TabIndex;
            control.TabStop = cfg.TabStop;
            control.Location = cfg.Location;
            control.Padding = cfg.Padding;
            control.Margin = cfg.Margin;
            control.Size = cfg.Size;
            control.Text = cfg.Text;
        }
        public void DestroyControl(FormBase parent, Control control) =>
            parent.Controls.Remove(control);

        public static TResult ShowModal<TForm, TResult>(IUISessionState sessionState)
            where TForm : ModalDialog<TResult>, IFormResult<TResult>, new()
        {
            using (var form = new TForm())
            {
                form.SessionState = sessionState;

                return form.ShowDialog() == DialogResult.OK ? form.Result : default;
            }
        }
    }
}
