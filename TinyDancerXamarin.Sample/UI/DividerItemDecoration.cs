using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;

namespace TinyDancerXamarin.Sample.UI
{
    /* You may obtain a copy of the License at
	 *
	 * http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */
    public class DividerItemDecoration : RecyclerView.ItemDecoration
    {

        private static readonly int[] Attrs = {
            Android.Resource.Attribute.ListDivider
        };

        public static readonly int HorizontalList = LinearLayoutManager.Horizontal;
        public static readonly int VerticalList = LinearLayoutManager.Vertical;

        private readonly Drawable mDivider;

        private int mOrientation;

        public DividerItemDecoration(Context context, int orientation)
        {
            var attributes = context.ObtainStyledAttributes(Attrs);
            mDivider = attributes.GetDrawable(0);
            attributes.Recycle();
            SetOrientation(orientation);
        }

        public void SetOrientation(int orientation)
        {
            if (orientation != HorizontalList && orientation != VerticalList)
            {
                throw new Java.Lang.IllegalArgumentException("invalid orientation");
            }
            mOrientation = orientation;
        }

        public override void OnDraw(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            if (mOrientation == VerticalList)
            {
                DrawVertical(c, parent);
            }
            else
            {
                DrawHorizontal(c, parent);
            }
        }

        public void DrawVertical(Canvas c, RecyclerView parent)
        {
            int left = parent.PaddingLeft;
            int right = parent.Width - parent.PaddingRight;

            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChildAt(i);
                var layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;
                int top = child.Bottom + layoutParams.BottomMargin;
                int bottom = top + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        public void DrawHorizontal(Canvas c, RecyclerView parent)
        {
            int top = parent.PaddingTop;
            int bottom = parent.Height - parent.PaddingBottom;

            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChildAt(i);
                var layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;
                int left = child.Right + layoutParams.RightMargin;
                int right = left + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        public override void GetItemOffsets(Rect outRect, Android.Views.View view, RecyclerView parent, RecyclerView.State state)
        {
            if (mOrientation == VerticalList)
            {
                outRect.Set(0, 0, 0, mDivider.IntrinsicHeight);
            }
            else
            {
                outRect.Set(0, 0, mDivider.IntrinsicWidth, 0);
            }
        }
    }
}


