using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Miru.Views
{
    // TODO: add unit tests
    public class TextBoxHelper : DependencyObject
    {
        public class MvvmCommand : DependencyObject, ICommand
        {
            readonly Action<object> _execute;
            readonly Func<object, bool> _canExecute;
            public event EventHandler CanExecuteChanged;
            public MvvmCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                if (execute == null) throw new ArgumentNullException("execute");
                _canExecute = canExecute == null ? parmeter => AlwaysCanExecute : canExecute;
                _execute = execute;
            }
            public object Tag
            {
                get { return GetValue(TagProperty); }
                set { SetValue(TagProperty, value); }
            }
            public static readonly DependencyProperty TagProperty = DependencyProperty.Register("Tag", typeof(object), typeof(MvvmCommand), new PropertyMetadata(null));
            const bool AlwaysCanExecute = true;
            public void EvaluateCanExecute()
            {
                EventHandler temp = CanExecuteChanged;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
            public virtual void Execute(object parameter)
            {
                _execute(parameter == null ? this : parameter);
            }
            public virtual bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }
        }

        public static KeyGesture GetFocusGesture(DependencyObject obj)
        {
            return (KeyGesture)obj.GetValue(FocusGestureProperty);
        }

        public static void SetFocusGesture(DependencyObject obj, KeyGesture value)
        {
            obj.SetValue(FocusGestureProperty, value);
        }

        [TypeConverter(typeof(KeyGestureConverter))]
        public static readonly DependencyProperty FocusGestureProperty =
            DependencyProperty.RegisterAttached("FocusGesture", typeof(KeyGesture), typeof(TextBoxHelper), new PropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control), new PropertyChangedCallback((s, e) =>
            {
                UIElement targetElement = s as UIElement;
                if (targetElement != null)
                {
                    MvvmCommand command = new MvvmCommand(parameter => TextBoxHelper.FocusCommand(parameter))
                    {
                        Tag = targetElement,
                    };
                    InputGesture inputg = (KeyGesture)e.NewValue;
                    (Window.GetWindow(targetElement)).InputBindings.Add(new InputBinding(command, inputg));
                }
            })));

        public static void FocusCommand(object parameter)
        {
            MvvmCommand targetCommand = parameter as MvvmCommand;
            if (targetCommand != null)
            {
                UIElement targetElement = targetCommand.Tag as UIElement;
                if (targetElement != null)
                {
                    targetElement.Focus();
                }
            }
        }
    }
}
