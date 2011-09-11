using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class LowerStatReqRune : BaseMinorRune
	{
		[Constructable]
		public LowerStatReqRune() : base()
		{
			BaseAmount = 1;
			MaxAmount = 100;
		}

		public LowerStatReqRune( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Target = new RuneTarget( this );
				from.SendMessage( "Select the item you wish to enhance." );
			}
			else
			{
				from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060435, this.BaseAmount.ToString() ); // lower requirements ~1_val~%
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}

		private class RuneTarget : Target
		{
			private BaseMinorRune m_Rune;

			public RuneTarget( BaseMinorRune rune ) : base( -1, false, TargetFlags.None )
			{
				m_Rune = rune;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				Item item = target as Item;
				Type type = item.GetType();

				if ( item is BaseArmor || item is BaseHat || item is BaseWeapon )
				{
					if ( Runescribing.GetProps( item ) >= 7 )
					{
						from.SendMessage( "This item cannot be enhanced any further" );
					}
					else if ( item.ChantSlots >= 3 )
					{
						from.SendMessage( "This item cannot handle any more enhancments." );
					}
					else if ( Runescribing.CheckBlacklist( type ) == true )
					{
						from.SendMessage( "This item cannot be enhanced." );
					}
					else
					{
						int value = m_Rune.BaseAmount;
						int max = m_Rune.MaxAmount;

						if ( item is BaseArmor )
						{
							BaseArmor i = item as BaseArmor;

							if ( i.ArmorAttributes.LowerStatReq + value <= max )
								i.ArmorAttributes.LowerStatReq += value;
							else
								i.ArmorAttributes.LowerStatReq = max;
						}

						if ( item is BaseHat )
						{
							BaseHat i = item as BaseHat;

							if ( i.ClothingAttributes.LowerStatReq + value <= max )
								i.ClothingAttributes.LowerStatReq += value;
							else
								i.ClothingAttributes.LowerStatReq = max;
						}

						if ( item is BaseWeapon )
						{
							BaseWeapon i = item as BaseWeapon;

							if ( i.WeaponAttributes.LowerStatReq + value <= max )
								i.WeaponAttributes.LowerStatReq += value;
							else
								i.WeaponAttributes.LowerStatReq = max;
						}

						item.ChantSlots += 1;
						m_Rune.Delete();
					}
				}
				else
				{
					from.SendMessage( "You cannot use this enhancement on that." );
				}
			}
		}
	}
}
