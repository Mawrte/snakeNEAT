using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Domains.SnakeGame.Core;
using System.Xml;

namespace SharpNeat.Domains.SnakeGame.Utils
{
    class SnakeUtils
    {

        static int _height = 20;
        static int _width = 20;
        static int _ticksBetweenFood = 2;
        static int _maxFood = 2;
        static int _startLen = 3;

        public static SimpleSnakeWorldParams GetParamsFromXml(XmlElement xmlConfig)
        {
            int height = XmlUtils.TryGetValueAsInt(xmlConfig, "Height") ?? _height;
            int width = XmlUtils.TryGetValueAsInt(xmlConfig, "Width") ?? _width;
            int ticksBetweenFood = XmlUtils.TryGetValueAsInt(xmlConfig, "TicksBetweenFood") ?? _ticksBetweenFood;
            int maxFood = XmlUtils.TryGetValueAsInt(xmlConfig, "MaxFood") ?? _maxFood;
            int startLen = XmlUtils.TryGetValueAsInt(xmlConfig, "SnakeStartingLength") ?? _startLen;

            return new SimpleSnakeWorldParams(height: height, width: width, ticksBetweenFood: ticksBetweenFood, maxFood: maxFood, startLen: startLen);
        }
        
        // Ex: collection.TakeLast(5);
        
    }

    static class SnakeExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}
