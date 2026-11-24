namespace Cat.Core
{
    // Player-facing wording for a hint. Pure strings (no UnityEngine) so it is unit-testable.
    // Unaccented Vietnamese on purpose: it matches the rest of the game's UI text ("Goi y", "Quay lai", ...)
    // and the static TMP font atlas. Change the wording here only; the view never builds strings itself.
    public static class HintText
    {
        public const string NoClearHint = "Chua co goi y ro rang. Thu dat meo o cho khac.";
        public const string PlaceBulb = "O dang duoc danh dau: chac chan co meo.";
        public const string AvoidBulb = "O dang duoc danh dau: chac chan khong co meo.";

        // hint == null means FindHint() found nothing clear to say.
        public static string Describe(HintMove hint)
        {
            if (hint == null) return NoClearHint;
            return hint.Type == HintType.PlaceBulb ? PlaceBulb : AvoidBulb;
        }
    }
}
