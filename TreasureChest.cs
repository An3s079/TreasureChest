using System;
using ItemAPI;
using Gungeon;
using System.Collections.Generic;
using UnityEngine;

namespace An3sGunJam
{
	class TreasureChest : GunBehaviour
	{
		public static void Add()
		{
			//ammonomicon stuff
			Gun gun = ETGMod.Databases.Items.NewGun("Treasure Chest", "chest");
			Game.Items.Rename("outdated_gun_mods:treasure_chest", "ans:treasure_chest");

			gun.gameObject.AddComponent<TreasureChest>();
			gun.SetShortDescription("MONEY!");
			gun.SetLongDescription("the chest of moneybeard III. \n\n said to contain riches, but built so tough that almost anything it fell on would die...");
			gun.SetupSprite(null, "chest_idle_001", 10);

			//gun stats
			gun.SetAnimationFPS(gun.shootAnimation, 24);
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);

			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0f;
			gun.DefaultModule.cooldownTime = 0.25f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.quality = PickupObject.ItemQuality.A;
			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.preventRotation = true;
			gun.muzzleFlashEffects = null;
			gun.SetBaseMaxAmmo(1);
			gun.encounterTrackable.EncounterGuid = "spapi says we dont need these, but i say spapi cant tell me what to do";
			gun.carryPixelOffset = new IntVector2(-4, 17);
			gun.barrelOffset.position = new Vector3(0, 0);
			gun.muzzleFlashEffects = null;

			//projectile stuff.
			ProjectileModule.ChargeProjectile chargeProjectile = new ProjectileModule.ChargeProjectile
			{
				Projectile = Instantiate(gun.DefaultModule.projectiles[0]),
				ChargeTime = 1f,
			};

			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProjectile };
			chargeProjectile.Projectile.gameObject.SetActive(false);
			chargeproj = chargeProjectile;
			//gives the gun a looping charge anim.
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 0;

			FakePrefab.MarkAsFakePrefab(chargeProjectile.Projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(chargeProjectile.Projectile);
			chargeProjectile.Projectile.baseData.damage = 300;
			chargeProjectile.Projectile.transform.parent = gun.barrelOffset;
			chargeProjectile.Projectile.baseData.range = 100f;
			chargeProjectile.Projectile.baseData.speed = 30f;
			chargeProjectile.Projectile.SetProjectileSpriteRight("chest_projectile_001", 30, 23);
			ETGMod.Databases.Items.Add(gun, null, "ANY");
		}
		private static ProjectileModule.ChargeProjectile chargeproj;
		private bool hasBeenUpgraded = false;
		private bool hasBeenUpgraded2 = false;
		public override void PostProcessProjectile(Projectile projectile)
		{
			base.PostProcessProjectile(projectile);
			var p = gun.CurrentOwner as PlayerController;
			//makes sure it doesnt subscibe all like, 5000 projectiles that are shot if the player has the what could be in here? synergy.
			if (p.PlayerHasActiveSynergy("what could be in here?"))
			{
				return;
			}

			projectile.ignoreDamageCaps = true;
			projectile.OnDestruction += this.onDestroyed;
			
		}

		private void onDestroyed(Projectile proj)
		{

			//plays the sound i made in my sound bank
			AkSoundEngine.PostEvent("Play_Wooden_Box_Break", GameManager.Instance.PrimaryPlayer.gameObject);

			var p = proj.Owner as PlayerController;

			//makes sure player doesnt have who needs money synergy.
			if (!p.PlayerHasActiveSynergy("who even needs money."))
			{
				//spawns 35 casings and 3 hegemony credit, 
				//the manual gives it the effect of lootbag, where they disapear over time.
				LootEngine.SpawnCurrencyManual(proj.sprite.WorldCenter, 35);
				LootEngine.SpawnCurrency(proj.sprite.WorldCenter, 3, true);
			}
			else
			{
				//cause explosion
				var data = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
				data.damageToPlayer = 0;
				data.damage = 5;
				Exploder.Explode(proj.sprite.WorldCenter, data, Vector2.zero, null, false, CoreDamageTypes.None, false);
			}

			//spawns random item at proj position if player has a certain synergy. 

			if (p.PlayerHasActiveSynergy("oh you can open that!"))
			{
				var itemtopick = UnityEngine.Random.Range(0, Module.items.Count);
				LootEngine.SpawnItem(Game.Items[Module.items[itemtopick]].gameObject, proj.sprite.WorldCenter, Vector2.zero, 0);
			}
		}

		public override void OnPostFired(PlayerController player, Gun gun)
		{
			gun.PreventNormalFireAudio = true;
		}

		private bool HasReloaded;

		protected void Update()
		{
			if (gun.CurrentOwner)
			{

				if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
				float FacingDirection = gun.CurrentOwner.FacingDirection;
				if (FacingDirection <= 89 && FacingDirection >= -89)
				{
					gun.carryPixelOffset = new IntVector2(-4, 17);
				}
				else
				{
					gun.carryPixelOffset = new IntVector2(12, 17);
				}
				//upgrades gun based on synergies.
				var p = gun.CurrentOwner as PlayerController;
				if (p.PlayerHasActiveSynergy("who even needs money."))
				{
					if (hasBeenUpgraded == false)
					{
						gun.SetBaseMaxAmmo(50);
						gun.CurrentAmmo = 50;
						gun.DefaultModule.chargeProjectiles[0].Projectile.baseData.damage = 10;
						hasBeenUpgraded = true;
					}
				}
				else
				{
					if (hasBeenUpgraded == true)
					{
						gun.SetBaseMaxAmmo(1);
						gun.CurrentAmmo = 1;
						gun.DefaultModule.chargeProjectiles[0].Projectile.baseData.damage = 300;
						hasBeenUpgraded = false;
					}

				}

				if(p.PlayerHasActiveSynergy("what could be in here?"))
				{
					if (hasBeenUpgraded2 == false)
					{
						//stolen from test gun
						gun.Volley.projectiles.Clear();
						gun.RuntimeModuleData.Clear();
						for (int i = 0; i < ETGMod.Databases.Items.Count; i++)
						{
							Gun other = ETGMod.Databases.Items[i] as Gun;
							if (other == null) continue; // Watch out for null entries in the database! Those are WIP / removed guns.
							ProjectileModule module = gun.AddProjectileModuleFrom(other, clonedProjectiles: false);
							// We don't want to eat more ammo than required.
							module.ammoCost = 0;
							module.numberOfShotsInClip = gun.CurrentAmmo;
							ModuleShootData moduleData = new ModuleShootData();
							gun.RuntimeModuleData[module] = moduleData;
						}
						hasBeenUpgraded2 = true;
					}
				}

			}
		}

		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				//AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
				//AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
			}
		}

	}
}
