using AppConfiguration;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs
{
    public class ClassForDBInstance
    {
        protected ConfigurationMgr configurationMgr;

        public ClassForDBInstance()
        {
            // configurationMgr의 객체 생성을 하지 않아도 사용가능
            // Instance() => public static으로 되어 있으므로
            configurationMgr = ConfigurationMgr.Instance();
        }
    }
}
