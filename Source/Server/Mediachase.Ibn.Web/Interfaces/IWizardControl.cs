using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Web.Interfaces
{
	public interface IWizardControl
	{
		bool ShowSteps { get; }
		int StepCount { get; }
		string TopTitle { get; }
		string Subtitle { get; }
		string MiddleButtonText { get; }
		string CancelText { get; }

		void SetStep(int stepNumber);
		string GenerateFinalStepScript();
		void CancelAction();
	}
}
