using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ProjectPaint
{
    public class ShapeFactory
    {
        public Dictionary<string, IShape> Prototypes =
            new Dictionary<string, IShape>();

        public IShape Create(String choice)
        {
            if (string.IsNullOrEmpty(choice) || !Prototypes.ContainsKey(choice))
            {
                // Trả về null hoặc một hình vẽ mặc định tùy thuộc vào yêu cầu của bạn.
                return null;
            }

            // Xử lý khi có choice hợp lệ.
            IShape shape = Prototypes[choice].Clone();
            return shape;
        }
    }
}
