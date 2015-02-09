using Sce.PlayStation.Core;

namespace TheATeam
{
	public class HealthWall: EntityAlive
	{
		public HealthWall(char key, Vector2 position): base(position)
		{
			Key = key;
			Stats.Health = 1;
			Stats.Lives = 3;
			Stats.MaxHealth = 1;
			Stats.MaxLives = 3;
		}
		
		override public void Update(float dt)
		{
			if(IsAlive)
			{
				if(Stats.Health > Stats.MaxHealth && Stats.Lives < Stats.MaxLives)
				{
					Stats.Lives++;
					Stats.Health = Stats.MaxHealth;
				}
				TileIndex2D.X = TileIndex2D.X + Stats.MaxLives - Stats.Lives;
			}
			else
			{
				Key = 'E';
				UnscheduleUpdate();
			}
			base.Update(dt);
		}

		public override void TakeDamage(char element='N', int damage=1)
		{
			if(IsAlive)
			{
				Stats.Health += damage * (element == Key ? 1 : -1);
			}
			base.TakeDamage(element, damage);
		}
	}
}