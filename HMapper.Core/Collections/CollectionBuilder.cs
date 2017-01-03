using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Metadata;
using System.Collections.ObjectModel;

namespace HMapper.Collections
{
    /// <summary>
    /// Will generate an expression that loops through the specified collection and maps the elements.
    /// The returned expression will be a Collection.
    /// </summary>
    internal class CollectionBuilder
    {
        /// <summary>
        /// Returns the expression that returns the mapped collection.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="targetCollType"></param>
        /// <param name="sourceCollType"></param>
        /// <param name="listExpr"></param>
        /// <param name="includeChain"></param>
        /// <param name="usedBuilders"></param>
        /// <returns></returns>
        public static Expression GetExpression(MapMode mapMode, Type targetCollType, Type sourceCollType, Expression listExpr, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            return BaseCollectionBuilder.GetExpression(typeof(Collection<>), mapMode, targetCollType, sourceCollType, listExpr, includeChain, usedBuilders);
        }
    }
}
