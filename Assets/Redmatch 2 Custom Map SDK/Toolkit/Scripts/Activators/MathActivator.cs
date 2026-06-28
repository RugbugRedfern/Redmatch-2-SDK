
using System.Collections.Generic;
using UnityEngine;

public class MathActivator : Activator
{
	public MathCalculation[] calculations = new MathCalculation[0];

	public override void RunErrorChecks(ref List<string> errors)
	{
		BaseErrorChecks(ref errors);
		if(calculations == null)
			return;

		for(int i = 0; i < calculations.Length; i++)
		{
			if(!calculations[i].firstValue.Valid())
			{
				errors.Add($"First calculation value source is not valid (Calculation #{i + 1}).");
			}
			if(!calculations[i].secondValue.Valid())
			{
				errors.Add($"Second calculation value source is not valid (Calculation #{i + 1}).");
			}
			if(!calculations[i].output.Valid())
			{
				errors.Add($"Output value source is not valid (Calculation #{i + 1}).");
			}
			if(calculations[i].output.sourceType == ValueSource.SourceType.Constant)
			{
				errors.Add("A calculation cannot output to a constant value.");
			}
			if(calculations[i].output.sourceType == ValueSource.SourceType.RoomInfo)
			{
				errors.Add("A calculation cannot output to RoomInfo.");
			}
			if(calculations[i].output.sourceType == ValueSource.SourceType.FromTriggerSource)
			{
				errors.Add("A calculation cannot output to FromTriggerSource.");
			}
		}
	}

#if REDMATCH
	protected override void InternalActivate(ActivatePayload payload)
	{
		for(int i = 0; i < calculations.Length; i++)
		{
			int val = Calculate(calculations[i], payload);
			if(calculations[i].output.sourceType == ValueSource.SourceType.HealthSyncer)
			{
				calculations[i].output.healthSyncer.SetHealth(val);
			}
			if(calculations[i].output.sourceType == ValueSource.SourceType.IntSyncer)
			{
				calculations[i].output.intSyncer.SetInt(val);
			}
		}
	}

	int Calculate(MathCalculation calculation, ActivatePayload payload)
	{
		switch(calculation.calculationType)
		{
			case MathCalculation.CalculationType.Add:
				return GetValue(calculation.firstValue, payload) + GetValue(calculation.secondValue, payload);
			case MathCalculation.CalculationType.Subtract:
				return GetValue(calculation.firstValue, payload) - GetValue(calculation.secondValue, payload);
			case MathCalculation.CalculationType.Multiply:
				return GetValue(calculation.firstValue, payload) * GetValue(calculation.secondValue, payload);
			case MathCalculation.CalculationType.Divide:
				if(GetValue(calculation.secondValue, payload) == 0)
					return 0;
				return GetValue(calculation.firstValue, payload) / GetValue(calculation.secondValue, payload);
			case MathCalculation.CalculationType.Exponent:
				return Mathf.RoundToInt(Mathf.Pow(GetValue(calculation.firstValue, payload), GetValue(calculation.secondValue, payload)));
			case MathCalculation.CalculationType.Root:
				return Mathf.RoundToInt(Mathf.Pow(GetValue(calculation.firstValue, payload), 1f / GetValue(calculation.secondValue, payload)));
			case MathCalculation.CalculationType.Remainder:
				return GetValue(calculation.firstValue, payload) % GetValue(calculation.secondValue, payload);
			case MathCalculation.CalculationType.LowestValue:
				return Mathf.Min(GetValue(calculation.firstValue, payload), GetValue(calculation.secondValue, payload));
			case MathCalculation.CalculationType.HighestValue:
				return Mathf.Max(GetValue(calculation.firstValue, payload), GetValue(calculation.secondValue, payload));
			case MathCalculation.CalculationType.RandomRange:
				return Random.Range(GetValue(calculation.firstValue, payload), GetValue(calculation.secondValue, payload) + 1);
			case MathCalculation.CalculationType.BooleanAND:
				return (GetBool(calculation.firstValue, payload) & GetBool(calculation.secondValue, payload)) ? 1 : 0;
			case MathCalculation.CalculationType.BooleanOR:
				return (GetBool(calculation.firstValue, payload) | GetBool(calculation.secondValue, payload)) ? 1 : 0;
			case MathCalculation.CalculationType.BooleanNAND:
				return !(GetBool(calculation.firstValue, payload) & GetBool(calculation.secondValue, payload)) ? 1 : 0;
			case MathCalculation.CalculationType.BooleanXOR:
				return (GetBool(calculation.firstValue, payload) ^ GetBool(calculation.secondValue, payload)) ? 1 : 0;
			case MathCalculation.CalculationType.BooleanNOR:
				return !(GetBool(calculation.firstValue, payload) | GetBool(calculation.secondValue, payload)) ? 1 : 0;
			case MathCalculation.CalculationType.BooleanXNOR:
				return !(GetBool(calculation.firstValue, payload) ^ GetBool(calculation.secondValue, payload)) ? 1 : 0;
		}

		return 0;
	}
#endif
}

[System.Serializable]
public class MathCalculation
{
	// Start boolean calculations at 20 to leave space for more arithmetic (just in case!)
	public enum CalculationType { Add, Subtract, Multiply, Divide, Exponent, Root, Remainder, LowestValue, HighestValue, RandomRange, BooleanAND = 20, BooleanOR, BooleanNAND, BooleanXOR, BooleanNOR, BooleanXNOR }
	public CalculationType calculationType;
	public ValueSource firstValue;
	public ValueSource secondValue;
	public ValueSource output;
}