using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen
    {
		private string _userName;


		public string UserName
		{
			get 
			{ 
				
				return _userName; 
			}
			set
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => SyncStatusText);
			}
		}

		public string SyncStatusText
		{
			get 
			{
				if (string.IsNullOrWhiteSpace(_userName))
				{
					return "Not synced";
				}
				return $"Synced to the { _userName }'s anime list."; 
			}
		}

	}
}
