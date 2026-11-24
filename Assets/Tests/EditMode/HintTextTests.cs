using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class HintTextTests
    {
        private static bool IsAscii(string s)
        {
            foreach (char c in s)
                if (c > 127) return false;
            return true;
        }

        [Test]
        public void NoHint_GivesAnExplanationNotEmptyText()
        {
            string text = HintText.Describe(null);
            Assert.IsNotEmpty(text);
            Assert.AreEqual(HintText.NoClearHint, text);
        }

        [Test]
        public void PlaceBulb_AndAvoidBulb_HaveDifferentMessages()
        {
            string place = HintText.Describe(new HintMove { X = 0, Y = 0, Type = HintType.PlaceBulb });
            string avoid = HintText.Describe(new HintMove { X = 0, Y = 0, Type = HintType.AvoidBulb });
            Assert.AreEqual(HintText.PlaceBulb, place);
            Assert.AreEqual(HintText.AvoidBulb, avoid);
            Assert.AreNotEqual(place, avoid);
        }

        [Test]
        public void AvoidMessage_SaysNoCat_PlaceMessage_DoesNot()
        {
            Assert.IsTrue(HintText.AvoidBulb.Contains("khong"));
            Assert.IsFalse(HintText.PlaceBulb.Contains("khong"));
        }

        [Test]
        public void Messages_FollowTheUnaccentedUiConvention()
        {
            Assert.IsTrue(IsAscii(HintText.NoClearHint));
            Assert.IsTrue(IsAscii(HintText.PlaceBulb));
            Assert.IsTrue(IsAscii(HintText.AvoidBulb));
        }

        [Test]
        public void MessageDoesNotLeakInternalReasonOrCoordinates()
        {
            HintMove hint = TestGrids.Level01Like().FindHint();
            string text = HintText.Describe(hint);
            Assert.IsFalse(text.Contains("(" + hint.X + "," + hint.Y + ")"));
            Assert.AreNotEqual(hint.Reason, text);
        }
    }
}
