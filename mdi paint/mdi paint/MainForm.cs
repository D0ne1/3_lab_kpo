using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PluginContracts;
using System.Reflection;


namespace mdi_paint
{
    public partial class MainForm : Form
    {

        
        private List<IImageFilter> loadedFilters = new List<IImageFilter>();



        /// <summary>
        /// Поле для храниния к файлу
        /// </summary>
        private string currentFilePath = null; // Поле для хранения пути к файлу
        /// <summary>
        /// Флаг закрашивания фигуры
        /// </summary>
        public static bool IsFilled { get; set; } = false;

        public static Tools CurrentTool {  get; set; }
        public static Color CurrentColor { get; set; }
        public static int CurrentWidth { get; set; }

        public MainForm()
        {
            InitializeComponent();
            CurrentColor = Color.Black;
            CurrentWidth = 1;

            this.MdiChildActivate += MainForm_MdiChildActivate;
            сохранитьToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
        }

        

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new FormAbout();
            frm.ShowDialog();
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = new FormDocument();
            doc.MdiParent = this;
            doc.Show();

        }

        private void redbutton_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Red;
        }

        private void greenbutton_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Green;
        }

        private void palettebutton_Click(object sender, EventArgs e)
        {
            var diolog = new ColorDialog();
            if (diolog.ShowDialog() == DialogResult.OK) { CurrentColor = diolog.Color; }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CurrentWidth = 5;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CurrentWidth = 10;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CurrentWidth = 1;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            var doc = ActiveMdiChild as FormDocument;
            if (doc != null)
            {
                if (string.IsNullOrEmpty(doc.currentFilePath))
                {
                    сохранитьToolStripMenuItem_Click(sender, e); // Вызываем "Сохранить как"
                }
                else
                {
                    doc.Save(); // Просто сохраняем в тот же файл
                }
            }
            
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            var dd = ActiveMdiChild as FormDocument;
            if (dd != null)
            {
                using (var dlg = new SaveFileDialog
                {
                    AddExtension = true,
                    Filter = "Windows Bitmap (*.bmp)|*.bmp|Файлы JPEG (*.jpg)|*.jpg",
                    DefaultExt = "bmp"
                })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        ImageFormat format = dlg.FilterIndex == 1 ? ImageFormat.Bmp : ImageFormat.Jpeg;
                        dd.SaveAs(dlg.FileName, format);
                    }
                }
            }

        }


        private void каскадToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void слеваНаправоToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        

        private void Pen_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Pen;
            UpdateCursor();
        }

        private void Circle_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Circle;
            UpdateCursor();
        }

        private void Arrow_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Arrow;
            UpdateCursor();
        }
        private void rubber_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Rubber;
            UpdateCursor();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Line;
            UpdateCursor();
        }
        private void txtbtn_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Text;
            UpdateCursor();
        }
        private void bucketbtn_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Bucket;
            UpdateCursor();
        }

        private void файлToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {

        }
        public void ShowPosition(int x, int y)
        {
            if (x != -1)
            {
                status_position.Text = $"X: {x} Y: {y}";
            }
            else
            {
                status_position.Text = string.Empty;
            }
        }
        public void ShowScale(float zoom)
        {
            btnScalePercentage.Text = $"{zoom:F0}%";  // Округляем до целого числа
        }

        private void рисунокToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            размерХолстаToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное окно — это форма документа
            if (ActiveMdiChild is FormDocument canvasForm)
            {
                using (CanvasSizeForm canvasSizeForm = new CanvasSizeForm())
                {
                    // Передаём текущие размеры холста в форму  
                    canvasSizeForm.CanvasWidth = canvasForm.CanvasWidth;
                    canvasSizeForm.CanvasHeight = canvasForm.CanvasHeight;

                    // Показываем форму как диалоговое окно
                    if (canvasSizeForm.ShowDialog() == DialogResult.OK)
                    {
                        // Получаем новые размеры и применяем их
                        int newWidth = canvasSizeForm.CanvasWidth;
                        int newHeight = canvasSizeForm.CanvasHeight;

                        canvasForm.ResizeCanvas(newWidth, newHeight);
                    }
                }
            }
        }

        private void fillbtn_Click(object sender, EventArgs e)
        {
            IsFilled = !IsFilled; // Переключаем режим заливки
        }
        public void UpdateCursor()
        {
            try
            {
                string path = "";
                switch (MainForm.CurrentTool)
                {
                    case Tools.Pen:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-ручка-24.cur";
                        break;
                    case Tools.Circle:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-круг-50.cur";
                        break;
                    case Tools.Arrow:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-arrow-64.cur";
                        break;
                    case Tools.Line:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-line-50.cur";
                        break;
                    case Tools.Rubber:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-ластик-30.cur";
                        break;
                    case Tools.Text:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-текст-30.cur";
                        break;
                    case Tools.Bucket:
                        path = @"C:\ДАНИИЛ\универ\2 курс\кпо\1 лаба\img\curs\icons8-ведро-с-водой-48.cur";
                        break;
                    default:
                        Cursor = Cursors.Default;
                        return;
                }

                Cursor = new Cursor(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки курсора: " + ex.Message);
                Cursor = Cursors.Cross; // Резервный курсор
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Все файлы (*.bmp;*.jpg;*.jpeg)|*.bmp;*.jpg;*.jpeg|BMP файлы (*.bmp)|*.bmp|JPEG файлы (*.jpg, *.jpeg)|*.jpg;*.jpeg";
                openFileDialog.Title = "Открыть изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap tempBmp;
                        using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            tempBmp = new Bitmap(stream); // Загружаем из потока, чтобы избежать блокировки файла
                        }

                        var doc = ActiveMdiChild as FormDocument;
                        if (doc == null)
                        {
                            doc = new FormDocument();
                            doc.MdiParent = this;
                            doc.Show();
                        }

                        doc.bmp?.Dispose(); // Освобождаем предыдущее изображение
                        doc.bmp = new Bitmap(tempBmp);
                        doc.bmpTemp = new Bitmap(tempBmp);
                        doc.currentFilePath = openFileDialog.FileName;
                        doc.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            var d = ActiveMdiChild as FormDocument;
            сохранитьToolStripMenuItem.Enabled = d != null;
            saveToolStripButton.Enabled = d != null;
        }

        private void scale_plus_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ZoomIn();
                Invalidate();
            }
        }

        private void scale_minus_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ZoomOut();
                Invalidate();
            }
        }

        private void btnScalePercentage_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Zoom100();
                Invalidate();
            }
        }

        private void status_position_Click(object sender, EventArgs e)
        {

        }
        private List<IImageFilter> filters;
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadPlugins();
            CreateFiltersMenu();
        }
        private void LoadPlugins()
        {
            loadedFilters.Clear();

            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!Directory.Exists(pluginsPath))
                Directory.CreateDirectory(pluginsPath);

            foreach (var dll in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    foreach (var type in asm.GetTypes())
                    {
                        if (typeof(IImageFilter).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                        {
                            var filter = (IImageFilter)Activator.CreateInstance(type);
                            loadedFilters.Add(filter);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Ошибка загрузки типов из {dll}:");
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        sb.AppendLine(loaderEx.Message);
                        if (loaderEx is FileNotFoundException fnfEx)
                        {
                            sb.AppendLine($"Не найден файл: {fnfEx.FileName}");
                        }
                    }
                    MessageBox.Show(sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки плагина {dll}: {ex.Message}");
                }
            }

           
        }
        
        private void CreateFiltersMenu()
        {
            // Очищаем старые пункты фильтров
            фильтрыToolStripMenuItem.DropDownItems.Clear();

            // Для каждого загруженного фильтра создаём пункт меню
            foreach (var filter in loadedFilters)
            {
                var item = new ToolStripMenuItem(filter.Name); // Имя фильтра как текст пункта
                item.ToolTipText = $"Автор: {filter.Author}, Версия: {filter.Version}";
                // Навешиваем обработчик нажатия
                item.Click += (s, e) => ApplyFilter(filter);
                фильтрыToolStripMenuItem.DropDownItems.Add(item);
            }

            // (Необязательно) Разделитель и пункт управления плагинами
            фильтрыToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var manageItem = new ToolStripMenuItem("Управление плагинами...");
            manageItem.Click += (s, e) => ShowPluginsDialog();
            фильтрыToolStripMenuItem.DropDownItems.Add(manageItem);
        }
        private void ShowPluginsDialog()
        {
            var dlg = new PluginManagerForm(loadedFilters); 
            dlg.ShowDialog();
        }
        private void ApplyFilter(IImageFilter plugin)
        {
            if (!(this.ActiveMdiChild is FormDocument doc))
            {
                MessageBox.Show("Нет активного изображения.");
                return;
            }

            try
            {
                Bitmap source = doc.bmp;
                Bitmap result = plugin.Apply(source);

                doc.bmp = result;
                doc.bmpTemp = new Bitmap(result);
                doc.Invalidate(); // Перерисовка формы
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения фильтра: {ex.Message}");
            }
        }

        private Bitmap bmp;
        private Bitmap bmpTemp;

        private void фильтрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        

      
    }
}