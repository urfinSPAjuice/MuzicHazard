using UnityEngine;
using System.Collections;

public class Filter {

	public enum FilterMode {
		LowPass = 0,
		HighPass,
		BandPass,
		NumFilterModes
	};

	public enum FilterOrder	{
		Firtst_6db_oct = 0,
		Second_12db_oct,
		Third_18db_oct,
		Fourth_24db_oct
	};

	private FilterMode mFilterMode;
	private FilterOrder mFilterOrder;
	private float cutoff;
	private float resonance;
	private float feedbackAmount;
	private float cutoffMod;
	private float buf0, buf1, buf2, buf3, f_out;

	public Filter () {
		mFilterMode = FilterMode.LowPass;
		mFilterOrder = FilterOrder.Fourth_24db_oct;
		cutoff = .99f;
		resonance = 0f;
		cutoffMod = 0f;
		buf0 = buf1 = buf2 = buf3 = 0f;
		CalculateFeedbackAmount();
	}

	public void Reset () {
		buf0 = buf1 = buf2 = buf3 = 0f;
	}

	public void SetFilterMode (FilterMode newMode) {
		mFilterMode = newMode;
	}

	public void SetFilterOrder (FilterOrder newOrder) {
		mFilterOrder = newOrder;
	}

	public void SetCutoff (float newCutoff) {
		cutoff = newCutoff;
		CalculateFeedbackAmount();
	}

	public void SetResonance (float newResonance) {
		resonance = newResonance;
		CalculateFeedbackAmount();
	}

	public void SetCutoffMod(float newCutoffMod) {
		cutoffMod = newCutoffMod;
		CalculateFeedbackAmount();
	}

	public float Process (float inputValue) {
		if (inputValue == 0f) return 0f;
		float calculatedCutoff = GetCalculatedCutoff();
		buf0 += calculatedCutoff * (inputValue - buf0 + feedbackAmount * (buf0 - buf1));
		buf1 += calculatedCutoff * (buf0 - buf1);
		buf2 += calculatedCutoff * (buf1 - buf2);
		buf3 += calculatedCutoff * (buf2 - buf3);

		switch (mFilterOrder) {
		case FilterOrder.Firtst_6db_oct:
			f_out = buf0;
			break;

		case FilterOrder.Second_12db_oct:
			f_out = buf1;
            break;

		case FilterOrder.Third_18db_oct:
			f_out = buf2;
            break;

		case FilterOrder.Fourth_24db_oct:
			f_out = buf3;
            break;
		}

		switch (mFilterMode) {
		case FilterMode.LowPass:
			return f_out;
		case FilterMode.HighPass:
			return inputValue - f_out;
		case FilterMode.BandPass:
			return buf0 - f_out;
		default:
			return 0f;
		}
	}

	private void CalculateFeedbackAmount() {
		feedbackAmount = resonance + resonance/(1f - GetCalculatedCutoff());
	}

	private float GetCalculatedCutoff() {
		return Mathf.Max(Mathf.Min(cutoff + cutoffMod, .99f), .01f);
	}
}
