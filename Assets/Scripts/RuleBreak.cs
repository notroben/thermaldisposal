public static class RuleBreak
{
    // Paperwork & Documentation
    public const string OutOfSequence = "INFRACTION: Documentation logged out of chronological sequence.";
    public const string FalsifiedWeigh = "INFRACTION: Falsified Records. Material logged as weighed prior to physical validation.";
    public const string FalsifiedBurn = "INFRACTION: Falsified Records. Material logged as incinerated prior to thermal disposal.";
    public const string UndocumentedBurn = "INFRACTION: Missing Documentation. Material incinerated prior to checklist logging.";
    public const string ScaleUnfinalized = "INFRACTION: Protocol Violation. Failed to finalize previous incineration record before processing new material on scale.";

    // Furnace & Equipment
    public const string UnweighedBurn = "INFRACTION: Protocol Violation. Un-weighed material inserted into thermal processing unit.";
    public const string FurnaceJammedDebris = "INFRACTION: Equipment Neglect. Attempted thermal processing while furnace was obstructed by carbonized debris.";
    public const string FurnaceJammedOverCapacity = "INFRACTION: Protocol Violation. Incineration of OVER_CAPACITY material resulted in permanent furnace jam.";
    public const string PropertyDestruction = "INFRACTION: Destruction of essential Company property.";

    // Shift
    public const string ShiftAbandonment = "INFRACTION: Shift Abandonment. Attempted to clock out prior to fulfilling daily quota and documentation requirements.";
}
