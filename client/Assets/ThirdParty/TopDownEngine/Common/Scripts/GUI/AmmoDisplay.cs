using UnityEngine;
using System.Collections;
using System.Text;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class that combines a progress bar and a text display
	/// and that can be used to display the current ammo level of a weapon
	/// </summary>
	[AddComponentMenu("TopDown Engine/GUI/AmmoDisplay")]
	public class AmmoDisplay : MMProgressBar 
	{
		/// the ID of the AmmoDisplay 
		[Tooltip("the ID of the AmmoDisplay ")]
		public int AmmoDisplayID = 0;
		/// the Text object used to display the current ammo numbers
		[Tooltip("the Text object used to display the current ammo numbers")]
		public Text TextDisplay;

		protected int _totalAmmoLastTime, _maxAmmoLastTime, _ammoInMagazineLastTime, _magazineSizeLastTime;
		protected StringBuilder _stringBuilder;

		/// <summary>
		/// On init we initialize our string builder
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();
			_stringBuilder = new StringBuilder();
		}
		
		/// <summary>
		/// Updates the text display with the parameter string
		/// </summary>
		/// <param name="newText">New text.</param>
		public virtual void UpdateTextDisplay(string newText)
		{
			if (TextDisplay != null)
			{
				TextDisplay.text = newText;
			}
		}

		/// <summary>
		/// Updates the ammo display's text and progress bar
		/// </summary>
		/// <param name="magazineBased">If set to <c>true</c> magazine based.</param>
		/// <param name="totalAmmo">Total ammo.</param>
		/// <param name="maxAmmo">Max ammo.</param>
		/// <param name="ammoInMagazine">Ammo in magazine.</param>
		/// <param name="magazineSize">Magazine size.</param>
		/// <param name="displayTotal">If set to <c>true</c> display total.</param>
		public virtual void UpdateAmmoDisplays(bool magazineBased, int totalAmmo, int maxAmmo, int ammoInMagazine, int magazineSize, bool displayTotal)
		{
			// we make sure there's actually something to update
			if ((_totalAmmoLastTime == totalAmmo)
			    && (_maxAmmoLastTime == maxAmmo)
			    && (_ammoInMagazineLastTime == ammoInMagazine)
			    && (_magazineSizeLastTime == magazineSize))
			{
				return;
			}

			_stringBuilder.Clear();
			
			if (magazineBased)
			{
				this.UpdateBar(ammoInMagazine,0,magazineSize);	
				if (displayTotal)
				{
					_stringBuilder.Append(ammoInMagazine.ToString());
					_stringBuilder.Append("/");
					_stringBuilder.Append(magazineSize.ToString());
					_stringBuilder.Append(" - ");
					_stringBuilder.Append((totalAmmo - ammoInMagazine).ToString());
					this.UpdateTextDisplay (_stringBuilder.ToString());					
				}
				else
				{
					_stringBuilder.Append(ammoInMagazine.ToString());
					_stringBuilder.Append("/");
					_stringBuilder.Append(magazineSize.ToString());
					this.UpdateTextDisplay (_stringBuilder.ToString());
				}
			}
			else
			{
				_stringBuilder.Append(totalAmmo.ToString());
				_stringBuilder.Append("/");
				_stringBuilder.Append(maxAmmo.ToString());
				this.UpdateBar(totalAmmo,0,maxAmmo);	
				this.UpdateTextDisplay (_stringBuilder.ToString());
			}

			_totalAmmoLastTime = totalAmmo;
			_maxAmmoLastTime = maxAmmo;
			_ammoInMagazineLastTime = ammoInMagazine;
			_magazineSizeLastTime = magazineSize;
		}
	}
}