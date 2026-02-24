using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Simplify.ReactiveUI.Domain;

[IViewFor<ReactiveObject>]
// ReSharper disable once PartialTypeWithSinglePart
public partial class ViewModel1
{
    private void Test() { }
}
