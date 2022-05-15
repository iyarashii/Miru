using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Miru.Views
{
    public class TextBoxHelper : DependencyObject
    {
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
                    MvvmCommand command = new MvvmCommand(parameter => FocusCommand(parameter))
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
