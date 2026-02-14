using Xunit;

namespace KKPinView.Tests;

/// <summary>
/// Test collection for tests that modify static KKPinviewConstant.
/// Runs sequentially to avoid race conditions when changing TotalPinTextFields etc.
/// </summary>
[CollectionDefinition("Constants")]
public class ConstantsTestCollection
{
}
