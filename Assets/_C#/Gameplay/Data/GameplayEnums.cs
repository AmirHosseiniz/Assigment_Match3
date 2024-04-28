using System;

namespace GameplayEnums
{
    namespace Cells
    {   
        public enum TypeEnum
        {
            empty = 0,
            hole = 1,
            dot = 2,
            block = 7,
        }
        
        public static class TypeEnumUtilities
        {
            public static bool UsesColor(TypeEnum type)
            {
                switch (type)
                {
                    case TypeEnum.dot:
                        return true;
                }
                return false;
            }
        }
    }
}