using System;
using System.Diagnostics.CodeAnalysis;

namespace Detergent.Templating
{
    public interface ITemplateSource
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetTemplate();
    }
}