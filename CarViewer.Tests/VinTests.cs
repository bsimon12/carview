using CarViewer.Data.Services;
using NUnit.Framework;

namespace CarViewer.Tests {
    public class VinTests {
        [Test]
        public void ValidateVin_InputNullOrWhiteSpace_ReturnsFalse() {
            var vinService = new VinService();

            Assert.IsFalse(vinService.ValidateVIN(string.Empty));
#pragma warning disable CS8625 // Disabling as this is a deliberate misuse of the interface to test sturdiness
            Assert.IsFalse(vinService.ValidateVIN(null));
#pragma warning restore CS8625
            Assert.IsFalse(vinService.ValidateVIN(" ".PadRight(15)));
        }

        [Test]
        public void ValidateVin_InvalidLength_ReturnsFalse() {
            var vinService = new VinService();

            Assert.IsFalse(vinService.ValidateVIN("1"));
            Assert.IsFalse(vinService.ValidateVIN("1A"));
            Assert.IsFalse(vinService.ValidateVIN("1A1A1A1A1A1A1A1A1A1A"));
        }

        [Test]
        public void ValidateVin_ValidVin_ReturnsTrue() {
            var vinService = new VinService();

            Assert.IsTrue(vinService.ValidateVIN("1A1A1A1A1A1A1A1A"));
        }
    }
}
