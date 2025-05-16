using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace mdi_paint
{
    public partial class FormDocument : Form
    {
        private string textToDraw = string.Empty;  // Переменная для хранения текста
        private Point textPosition = Point.Empty;  // Переменная для хранения позиции текста

        public float scaleFactor = 1.0f; // Масштаб изображения
        private const float scaleStep = 0.1f; // Шаг увеличения/уменьшения
        private const float maxScale = 3.0f; // Максимальный масштаб
        private const float minScale = 0.5f; // Минимальный масштаб


        private Bitmap bitmap;

        public Bitmap Bmp
        {
            get { return bitmap; }
            set { bitmap = value; }
        }


        private bool isModified = false;  // Флаг, который отслеживает изменения
        public string currentFilePath; // Путь к текущему файлу
        public int CanvasWidth
        {
            get { return bmp.Width; }
            set
            {
                // Если нужно, создаём новый Bitmap с изменёнными размерами
                var newBmp = new Bitmap(value, bmp.Height);
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    g.DrawImage(bmp, 0, 0); // Копируем старое изображение
                }
                bmp = newBmp;
                bmpTemp = (Bitmap)bmp.Clone();
                Invalidate();
            }
        }

        public int CanvasHeight
        {
            get { return bmp.Height; }
            set
            {
                // Если нужно, создаём новый Bitmap с изменёнными размерами
                var newBmp = new Bitmap(bmp.Width, value);
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    g.DrawImage(bmp, 1000, 1000); // Копируем старое изображение
                }
                bmp = newBmp;
                bmpTemp = (Bitmap)bmp.Clone();
                Invalidate();
            }
        }

        int oldX, oldY;
        /// <summary>
        /// Битовая карта
        /// </summary>
        public Bitmap bmp = new Bitmap(1000,1000);
        public Bitmap bmpTemp;

        private MainForm _mainform;
        public FormDocument()
        {
            InitializeComponent();
            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);

            bmpTemp = bmp;
            this.BackColor = Color.White;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Заполняем холст белым цветом
                g.Clear(Color.White);
            }

        }

        private void FormDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                oldX = e.X; oldY = e.Y;
            }
            MarkAsModified();
        }

        private void FormDocument_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pen = new Pen(MainForm.CurrentColor, MainForm.CurrentWidth);
                switch (MainForm.CurrentTool)
                {
                    case Tools.Pen:
                        var g = Graphics.FromImage(bmp);
                        g.DrawLine(pen, oldX, oldY, e.X, e.Y);
                        oldX = e.X; oldY = e.Y;
                        bmpTemp = bmp;
                        Invalidate();
                        break;

                    case Tools.Circle:
                        bmpTemp = (Bitmap)bmp.Clone();
                        using (Graphics gr = Graphics.FromImage(bmpTemp))
                        {
                            Rectangle rect = new Rectangle(oldX, oldY, e.X - oldX, e.Y - oldY);
                            if (MainForm.IsFilled) // isFilled — булево значение, задающее закраску
                            {
                                using (SolidBrush brush = new SolidBrush(MainForm.CurrentColor))
                                {
                                    gr.FillEllipse(brush, rect);
                                }
                            }
                            else
                            {
                                using (pen)
                                {
                                    gr.DrawEllipse(pen, rect);
                                }
                            }
                        }
                        Invalidate();
                        break;

                    case Tools.Arrow:
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);

                        // Координаты начала и конца стрелки
                        float startX = oldX;
                        float startY = oldY;
                        float endX = e.X;
                        float endY = e.Y;

                        // Ширина и длина частей стрелки
                        float arrowBaseWidth = 20; // Ширина прямоугольной части
                        float arrowHeadWidth = 40; // Ширина наконечника
                        float arrowHeadLength = 30; // Длина наконечника

                        // Угол направления стрелки
                        double angle = Math.Atan2(endY - startY, endX - startX);

                        // Вычисление точек для прямоугольной основы
                        PointF rectP1 = new PointF(
                            startX + (float)(arrowBaseWidth / 2 * Math.Sin(angle)),
                            startY - (float)(arrowBaseWidth / 2 * Math.Cos(angle))
                        );
                        PointF rectP2 = new PointF(
                            startX - (float)(arrowBaseWidth / 2 * Math.Sin(angle)),
                            startY + (float)(arrowBaseWidth / 2 * Math.Cos(angle))
                        );
                        PointF rectP3 = new PointF(
                            endX - (float)(arrowHeadLength * Math.Cos(angle)) - (float)(arrowBaseWidth / 2 * Math.Sin(angle)),
                            endY + (float)(arrowBaseWidth / 2 * Math.Cos(angle)) - (float)(arrowHeadLength * Math.Sin(angle))
                        );
                        PointF rectP4 = new PointF(
                            endX - (float)(arrowHeadLength * Math.Cos(angle)) + (float)(arrowBaseWidth / 2 * Math.Sin(angle)),
                            endY - (float)(arrowBaseWidth / 2 * Math.Cos(angle)) - (float)(arrowHeadLength * Math.Sin(angle))
                        );

                        // Вычисление точек для треугольного наконечника
                        PointF tip = new PointF(endX, endY);
                        PointF headP1 = new PointF(
                            endX - (float)(arrowHeadLength * Math.Cos(angle)) - (float)(arrowHeadWidth / 2 * Math.Sin(angle)),
                            endY + (float)(arrowHeadWidth / 2 * Math.Cos(angle)) - (float)(arrowHeadLength * Math.Sin(angle))
                        );
                        PointF headP2 = new PointF(
                            endX - (float)(arrowHeadLength * Math.Cos(angle)) + (float)(arrowHeadWidth / 2 * Math.Sin(angle)),
                            endY - (float)(arrowHeadWidth / 2 * Math.Cos(angle)) - (float)(arrowHeadLength * Math.Sin(angle))
                        );

                        if (MainForm.IsFilled)
                        {
                            using (SolidBrush brush = new SolidBrush(MainForm.CurrentColor))
                            {
                                PointF[] arrowBody = { rectP1, rectP2, rectP3, rectP4 };
                                PointF[] arrowHead = { tip, headP1, headP2 };

                                g.FillPolygon(brush, arrowBody);
                                g.FillPolygon(brush, arrowHead);
                            }
                        }
                        else
                        {
                            g.DrawLine(pen, rectP1, rectP2);
                            g.DrawLine(pen, rectP2, rectP3);
                            g.DrawLine(pen, rectP3, rectP4);
                            g.DrawLine(pen, rectP4, rectP1);

                            g.DrawLine(pen, tip, headP1);
                            g.DrawLine(pen, tip, headP2);
                            g.DrawLine(pen, headP1, headP2);
                        }

                        Invalidate();
                        break;

                    case Tools.Line:
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        g.DrawLine(pen, new Point(oldX, oldY), new Point(e.X, e.Y));
                        Invalidate();
                        break;

                    case Tools.Rubber:
                        var s = Graphics.FromImage(bmp);
                        s.DrawLine(new Pen(Color.White, MainForm.CurrentWidth), oldX, oldY, e.X, e.Y);
                        oldX = e.X; oldY = e.Y;
                        bmpTemp = bmp;
                        Invalidate();
                        break;

                    case Tools.Text:
                        string text = ForTextcs.ShowDialog("Введите текст:", "Текст");
                        if (!string.IsNullOrEmpty(text))
                        {
                            textToDraw = text;  // Сохраняем введённый текст
                            textPosition = new Point(e.X, e.Y);  // Сохраняем позицию для текста
                            Invalidate();  // Перерисовываем форму
                        }
                        break;

                    case Tools.Bucket:
                        FillArea(e.Location, MainForm.CurrentColor);
                        Invalidate();
                        break;
                }
                var parent = MdiParent as MainForm;
                parent?.ShowPosition(e.X, e.Y); //когда не null
            }
        }

        private void FillArea(Point point, Color newColor)
        {
            // Получаем текущий цвет пикселя в точке клика
            Color oldColor = bmp.GetPixel(point.X, point.Y);

            // Если старый и новый цвет совпадают, выходим
            if (oldColor.ToArgb() == newColor.ToArgb()) return;

            // Используем очередь для обхода всех смежных пикселей
            Queue<Point> pixels = new Queue<Point>();
            pixels.Enqueue(point);

            // Начинаем обход
            while (pixels.Count > 0)
            {
                Point p = pixels.Dequeue();

                // Проверяем, что пиксель внутри границ изображения
                if (p.X < 0 || p.X >= bmp.Width || p.Y < 0 || p.Y >= bmp.Height)
                    continue;

                // Если пиксель совпадает по цвету с начальным, меняем его
                if (bmp.GetPixel(p.X, p.Y) == oldColor)
                {
                    bmp.SetPixel(p.X, p.Y, newColor);

                    // Добавляем смежные пиксели в очередь
                    pixels.Enqueue(new Point(p.X + 1, p.Y));
                    pixels.Enqueue(new Point(p.X - 1, p.Y));
                    pixels.Enqueue(new Point(p.X, p.Y + 1));
                    pixels.Enqueue(new Point(p.X, p.Y - 1));
                }
            }

            // Перерисовываем форму, чтобы отобразить изменения
            Invalidate();
        }



        private void FormDocument_Load(object sender, EventArgs e)
        {

        }

        private void FormDocument_MouseUp(object sender, MouseEventArgs e)
        {
            switch(MainForm.CurrentTool)
            {

                case Tools.Circle:
                    bmp = bmpTemp;
                    Invalidate();
                    break;
                case Tools.Arrow:
                    bmp = bmpTemp;
                    Invalidate();
                    break;
                case Tools.Line:
                    bmp = bmpTemp;
                    Invalidate();
                    break;
                case Tools.Rubber:
                    bmp = bmpTemp;
                    Invalidate();
                    break;

            }

        }
        
        protected override void OnPaint(PaintEventArgs e)
        {

           
            base.OnPaint(e);

            // Масштабируем изображение при рисовании
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Расчет нового размера изображения
            int newWidth = (int)(bmp.Width * scaleFactor);
            int newHeight = (int)(bmp.Height * scaleFactor);

            // Вместо вычитания смещения, вычисляем, где будет располагаться верхний левый угол изображения
            int offsetX = (int)(oldX - (oldX * scaleFactor));
            int offsetY = (int)(oldY - (oldY * scaleFactor));

            // Рисуем изображение с учетом смещения и масштаба
            e.Graphics.DrawImage(bmp, offsetX, offsetY, newWidth, newHeight);

            // Рисуем промежуточное изображение, если оно существует
            if (bmpTemp != null)
            {
                e.Graphics.DrawImage(bmpTemp, offsetX, offsetY, newWidth, newHeight);
            }

            // Рисуем текст, если был введён
            if (!string.IsNullOrEmpty(textToDraw))
            {
                // Выбираем шрифт и цвет для текста
                using (Font font = new Font("Arial", 12))
                {
                    using (Brush brush = new SolidBrush(MainForm.CurrentColor))
                    {
                        e.Graphics.DrawString(textToDraw, font, brush, textPosition);
                    }
                }
            }


        }

        private void FormDocument_MouseLeave(object sender, EventArgs e)
        {
            var parent = MdiParent as MainForm;
            parent?.ShowPosition(-1,-1);
        }

        public void ResizeCanvas(int width, int height)
        {
            // Создаём новый Bitmap с указанными размерами
            Bitmap newBmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newBmp))
            {
                // Копируем содержимое старого Bitmap в новый
                g.DrawImage(bmp, 0, 0);
            }

            ClientSize = new Size(width, height);
            // Заменяем старый Bitmap новым
            bmp = newBmp;
            bmpTemp = bmp;
            Invalidate();
        }
        public void Save()
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                ImageFormat format = currentFilePath.EndsWith(".bmp") ? ImageFormat.Bmp : ImageFormat.Jpeg;
                SaveAs(currentFilePath, format);
            }
            else
            {
                MessageBox.Show("Ошибка: Файл ещё не сохранён, используйте 'Сохранить как'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void SaveAs(string filePath, ImageFormat format)
        {

            if (bmp != null)
            {
                bmp.Save(filePath, format);
                currentFilePath = filePath; // Запоминаем путь
            }
            else
            {
                MessageBox.Show("Ошибка: Нет изображения для сохранения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void MarkAsModified()
        {
            isModified = true;
        }

        private void FormDocument_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            
            if (isModified) // Если файл был изменен
            {
                var result = MessageBox.Show("Хотите сохранить изменения?", "Сохранить изменения", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(currentFilePath)) // Если путь пустой, выводим ошибку и отменяем закрытие
                    {
                        MessageBox.Show("Ошибка: Файл ещё не сохранён, используйте 'Сохранить как'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true; // Отменяем закрытие формы
                        return;
                    }

                    Save(); // Сохранение файла
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Отмена закрытия
                }
            }

        }
        public void ZoomIn()
        {
            if (scaleFactor < maxScale)
            {
                scaleFactor += scaleStep;
                var parent = MdiParent as MainForm;
                float scalePercentage = scaleFactor * 100;  // Получаем процент от 100%
                parent?.ShowScale(scalePercentage); //когда не null
                Invalidate();
            }
        }
        
        public void ZoomOut()
        {   
            if (scaleFactor > minScale)
            {
                scaleFactor -= scaleStep;
                var parent = MdiParent as MainForm;
                float scalePercentage = scaleFactor * 100;  // Получаем процент от 100%
                parent?.ShowScale(scalePercentage); //когда не null
                //UpdateScalePercentage();
                Invalidate();
            }
        }
        public void Zoom100()
        {
            scaleFactor = 1.0f;
            var parent = MdiParent as MainForm;
            parent?.ShowScale(scaleFactor*100); //когда не null
            Invalidate();
        }
    }
}