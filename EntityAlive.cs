using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum AttackStatus
	{
		None = 0,
		MeleeNormal,
		MeleeStrong,
		RangedNormal,
		RangedStrong
	}
	
	public class Statistics
	{
		public int MaxHealth = 100;
		public int MaxLives = 2;
		public int Health = 100;
		public int Lives = 2;
		public int Defense = 100;
		public int Attack = 10;
		public int RangedAttack = 15;

		public double Luck { get { return Info.Rnd.NextDouble() * (1 - 0.5) + 0.5; } }
	}
	
	public class EntityAlive: Tile
	{
		protected AttackStatus attackState = AttackStatus.None;
		protected Vector2 positionDelta = new Vector2();
		protected Vector2i animationRangeX = new Vector2i();
		
		public Statistics Stats { get; set; }

		public EntityAlive Opponent { get; set; }

		public bool IsAlive { get; protected set; }
		
		public bool IsBoss { get; set; }

		public bool IsDefending { get; set; }

		public bool InBattle { get; set; }

		public int Damage { get { return CalculateDamage(attackState); } }

		public int DamageReceived { get; protected set; }
		
		public EntityAlive(Vector2 position):
			base(position)
		{
			// Make Stats
			Stats = new Statistics();
			// Alive by default
			IsAlive = true;
			// Collidable by default
			IsCollidable = true;
			// Not a boss by default
			IsBoss = false;
			// NPCs don't block by default
			IsDefending = false;
			// Attach attack timer
			if(this.GetType() == typeof(EntityAlive))
			{
				ScheduleInterval((dt) => {
					if(InBattle)
					{
						attackState = RandomAttack();
						if(Info.Rnd.NextDouble() <= 0.1)
							IsDefending = true;
					}
				}, 1f);
			}
		}
		
		public EntityAlive(int spriteIndexY, Vector2 position, Vector2i animationRangeX, float interval=0.2f):
			this(position)
		{
			// Assign variables
			this.animationRangeX = animationRangeX;
			TileIndex2D = new Vector2i(animationRangeX.X, spriteIndexY);
			// Attach custom animation function
			ScheduleInterval((dt) => {
				if(IsAlive)
				{
					int newTileIndex = TileIndex2D.X < animationRangeX.Y ? TileIndex2D.X + 1 : animationRangeX.X;
					TileIndex2D.X = newTileIndex;
				}
				else
				{
					TileIndex2D.X = TextureInfo.NumTiles.X - 1;
					UnscheduleUpdate();
				}
			}, interval);
		}
		
		public EntityAlive(Vector2i tileIndex2D, Vector2 position):
			this(position)
		{
			TileIndex2D = tileIndex2D;
		}
		
		override public void Update(float dt)
		{
			positionDelta = Vector2.Zero;
			// Dying
			if(Stats.Health <= 0)
			{
				Stats.Health = Stats.MaxHealth;
				Stats.Lives--;
				if(Stats.Lives <= 0)
					IsAlive = false;
			}
			// Fight!
			if(InBattle)
			{
				// Deal damage
				if(Opponent != null)
				{
					Opponent.DamageReceived = Damage;
					// Enemy killed
					if(!Opponent.IsAlive)
					{
						InBattle = false;
						Opponent.InBattle = false;
						Opponent.Opponent = null;
						Opponent = null;
						SceneManager.ReplaceUIScene();
					}
				}
				
				// Take damage
				if(DamageReceived > 0)
				{
					if(IsDefending && Stats.Defense > 0 && DamageReceived <= Stats.Defense)
						Stats.Defense -= DamageReceived;
					else
						Stats.Health -= DamageReceived;
					
					DamageReceived = 0;
				}
				// Reset attack, defense
				attackState = AttackStatus.None;
				IsDefending = false;
			}
			base.Update(dt);
		}
		
		private int CalculateDamage(AttackStatus attack)
		{
			int damage = 0;
			switch(attack)
			{
			case AttackStatus.None:
				break;
			case AttackStatus.MeleeNormal:
				damage = (int)(Stats.Attack * Stats.Luck);
				break;
			case AttackStatus.MeleeStrong:
				damage = (int)(Stats.Attack * Stats.Luck * 1.15);
				break;
			case AttackStatus.RangedNormal:
				damage = (int)(Stats.RangedAttack * Stats.Luck);
				break;
			case AttackStatus.RangedStrong:
				damage = (int)(Stats.RangedAttack * Stats.Luck * 1.15);
				break;
			default:
				break;
			}
			return damage;
		}
		
		public AttackStatus RandomAttack()
		{
			AttackStatus attack = (AttackStatus)Info.Rnd.Next(1, (int)AttackStatus.RangedStrong + 1);
			return attack;
		}
	}
}

