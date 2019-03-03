#region Copyright & License Information
/*
 * Copyright 2007-2019 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using OpenRA.Activities;
using OpenRA.Traits;
using System.Collections.Generic;

namespace OpenRA.Mods.Common.Traits
{
	[Desc("Unit must deploy before it can attack.")]
	public class AttackDeployInfo : AttackFrontalInfo, Requires<GrantConditionOnDeployInfo>
	{
		public override object Create(ActorInitializer init) { return new AttackDeploy(init.Self, this); }
	}

	public class AttackDeploy : AttackFrontal, INotifyDeployComplete
	{
		readonly AttackDeployInfo info;

		bool deployAttackIssued = false;
		bool deployForceAttack;
		Target deployTarget;

		public AttackDeploy(Actor self, AttackDeployInfo info)
			: base(self, info)
		{
			this.info = info;
		}

		void INotifyDeployComplete.FinishedDeploy(Actor self)
		{
			if (deployAttackIssued)
			{
				self.World.IssueOrder(new Order(deployForceAttack ? "ForceAttack" : "Attack", self, deployTarget, true));
				deployAttackIssued = false;
			}
		}

		void INotifyDeployComplete.FinishedUndeploy(Actor self)
		{
		}

		public override Activity GetAttackActivity(Actor self, Target newTarget, bool allowMove, bool forceAttack)
		{
			deployAttackIssued = true;
			deployForceAttack = forceAttack;
			deployTarget = newTarget;
			return base.GetAttackActivity(self, newTarget, allowMove, forceAttack);
		}

		public override void DoAttack(Actor self, Target target, IEnumerable<Armament> armaments = null)
		{
			self.World.IssueOrder(new Order("GrantConditionOnDeploy", self, false));
		}

	}
}
