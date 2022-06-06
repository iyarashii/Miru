// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Miru.Views
{
    public class MvvmCommand : DependencyObject, ICommand
    {
        readonly Action<object> _execute;
        internal readonly Func<object, bool> _canExecute;
        public event EventHandler CanExecuteChanged;
        public MvvmCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute ?? (parmeter => AlwaysCanExecute);
        }
        [ExcludeFromCodeCoverage]
        public object Tag
        {
            get { return GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }
        public static readonly DependencyProperty TagProperty = DependencyProperty
            .Register("Tag", typeof(object), typeof(MvvmCommand), new PropertyMetadata(null));
        const bool AlwaysCanExecute = true;

        [ExcludeFromCodeCoverage]
        public void EvaluateCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public virtual void Execute(object parameter)
        {
            _execute(parameter ?? this);
        }
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }
    }
}
