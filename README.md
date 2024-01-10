## I. Họ tên và mã số sinh viên các thành viên trong nhóm:
    + 21120093 - Trần Anh Kiệt
    + 21120105 - Trương Thành Nhân
    + 21120144 - Phạm Phúc Thuần

## II. Các chức năng đã thực hiện:
  ### a) Core requirements 
      1. Dynamically load all graphic objects that can be drawn from external DLL files
      2. The user can choose which object to draw
      3. The user can see the preview of the object they want to draw
      4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects
      5. The list of drawn objects can be saved and loaded again for continuing later. You must save in your own defined binary format.    
      6. Save and load all drawn objects as an image in png format
  ### b) Basic graphic objects
      1. Line: controlled by two points, the starting point, and the endpoint
      2. Rectangle: controlled by two points, the left top point, and the right bottom point
      3. Ellipse: controlled by two points, the left top point, and the right bottom point
      4. Text
      5. Brush
  ### c) Improvements
      1. Allow the user to change the color, pen width, stroke type (dash, dot, dash dot dot...)
      2. Adding text to the list of drawable objects      
      3. Adding image to the canvas
      4. Reduce flickering when drawing preview by using buffer to redraw all the canvas. Upgrade: Only redraw the needed region, no fullscreen redraw
      5. Zooming (Zoom in, zoom out)
      6. Undo, Redo (Command)
      7. Use Visitor design pattern (Suggestion: save objects into text file / xml file / json file / binary file)
      8. Allow the user to draw by fill or no fill with color
      Các chức năng ngoài gợi ý của thầy:
      9. Change size of drawing area
      10. Adding brush drawing
## III. Các chức năng chưa thực hiện:
      1. Adding Layers support
      2. Select a single element for editing again. Transforming horizontally and vertically. Rotate the image. Drag & Drop
      3. Cut / Copy / Paste
      4. Fill color by boundaries

## IV. Các chức năng giáo viên nên xem xét cộng điểm vì đã bỏ nhiều thời gian và công sức tìm hiểu:
      1. Adding text to the list of drawable objects
      2. Adding brush drawing
      3. Undo, Redo (Command)

## V. Điểm tự đánh giá cho từng thành viên:
    + 21120093 - Trần Anh Kiệt - 10 điểm
    + 21120105 - Trương Thành Nhân - 10 điểm
    + 21120144 - Phạm Phúc Thuần - 10 điểm

## VI. Link video: 


## VII. Một số lưu ý:
- .NET Framework 8.0
- Nếu chương trình build thành công nhưng không hiển thị màn hình Paint thì copy thư mục Img vào cùng cấp với ProjectPaint.exe
