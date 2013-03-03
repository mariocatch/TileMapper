using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TileMapper
{
    /// <summary>
    /// Main entry point Window for the TileMapper application.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const int MinRowsAndColumns = 12;
        private const int MaxRowsAndColumns = 64;
        private const int PixelOffset = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            mPlacedTiles = new ObservableCollection<Tile>();
            mAvailableTiles = new ObservableCollection<Tile>();
            SelectedColumnDimensions = SelectedRowDimensions = 48;
            Scale = 32;
            DataContext = this;
        }

        #region Settings Properties

        /// <summary>
        /// Gets or sets the available tile scaling.
        /// </summary>
        public int Scale
        {
            get { return mScale; }
            set
            {
                if (mScale != value)
                {
                    mScale = value;
                    RaisePropertyChanged("Scale");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sprite sheet path.
        /// </summary>
        public string SpriteSheetPath
        {
            get { return mSpriteSheetPath; }
            set
            {
                if (mSpriteSheetPath != value)
                {
                    mSpriteSheetPath = value;
                    RaisePropertyChanged("SpriteSheetPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected column dimensions.
        /// </summary>
        public int SelectedColumnDimensions
        {
            get { return mSelectedColumnDimensions; }
            set
            {
                if (mSelectedColumnDimensions != value)
                {
                    mSelectedColumnDimensions = value;
                    RaisePropertyChanged("SelectedColumnDimensions");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected row dimensions.
        /// </summary>
        public int SelectedRowDimensions
        {
            get { return mSelectedRowDimensions; }
            set
            {
                if (mSelectedRowDimensions != value)
                {
                    mSelectedRowDimensions = value;
                    RaisePropertyChanged("SelectedRowDimensions");
                }
            }
        }

        /// <summary>
        /// Gets or sets the set column dimensions.
        /// </summary>
        public int SetColumnDimensions
        {
            get { return mSetColumnDimensions; }
            set
            {
                if (mSetColumnDimensions != value)
                {
                    mSetColumnDimensions = value;
                    RaisePropertyChanged("SetColumnDimensions");
                }
            }
        }

        /// <summary>
        /// Gets or sets the set row dimensions.
        /// </summary>
        public int SetRowDimensions
        {
            get { return mSetRowDimensions; }
            set
            {
                if (mSetRowDimensions != value)
                {
                    mSetRowDimensions = value;
                    RaisePropertyChanged("SetRowDimensions");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected available tile.
        /// </summary>
        public Tile SelectedAvailableTile
        {
            get { return mSelectedAvailableTile; }
            set
            {
                if (mSelectedAvailableTile != value)
                {
                    mSelectedAvailableTile = value;
                    RaisePropertyChanged("SelectedAvailableTile");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected placed tile.
        /// </summary>
        public Tile SelectedPlacedTile
        {
            get { return mSelectedPlacedTile; }
            set
            {
                if (mSelectedPlacedTile != value)
                {
                    mSelectedPlacedTile = value;
                    RaisePropertyChanged("SelectedPlacedTile");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not we're in erase mode.
        /// </summary>
        public bool IsInEraseMode
        {
            get { return mIsInEraseMode; }
            set
            {
                if (mIsInEraseMode != value)
                {
                    mIsInEraseMode = value;
                    RaisePropertyChanged("IsInEraseMode");
                }
            }
        }

        #endregion

        #region Command Bindings

        /// <summary>
        /// Gets the available dimensions for the tile map grid.
        /// </summary>
        public IReadOnlyCollection<int> AvailableDimensionNumbers
        {
            get { return Enumerable.Range(MinRowsAndColumns, (MaxRowsAndColumns - MinRowsAndColumns) + 1).ToList().AsReadOnly(); }
        }

        /// <summary>
        /// Gets the command for applying settings.
        /// </summary>
        public ICommand ApplySettingsCommand
        {
            get { return mApplySettingsCommand ?? (mApplySettingsCommand = new RelayCommand(ApplySettings)); }
        }

        /// <summary>
        /// Gets the command for saving settings.
        /// </summary>
        public ICommand SaveAsCommand
        {
            get { return mSaveAsCommand ?? (mSaveAsCommand = new RelayCommand(SaveTileMap)); }
        }

        /// <summary>
        /// Gets the load sprite sheet command.
        /// </summary>
        public ICommand LoadSpriteSheetCommand
        {
            get { return mLoadSpriteSheetCommand ?? (mLoadSpriteSheetCommand = new RelayCommand(LoadSpriteSheet)); }
        }

        #endregion

        /// <summary>
        /// Gets the placed tiles in the grid.
        /// </summary>
        public ObservableCollection<Tile> PlacedTiles
        {
            get { return mPlacedTiles; }
            private set { mPlacedTiles = value; }
        }

        /// <summary>
        /// Gets the available tiles.
        /// </summary>
        public ObservableCollection<Tile> AvailableTiles
        {
            get { return mAvailableTiles; }
            private set { mAvailableTiles = value; }
        }

        /// <summary>
        /// Applies grid settings.
        /// </summary>
        private void ApplySettings()
        {
            mPlacedTiles.Clear();
            SetColumnDimensions = SelectedColumnDimensions;
            SetRowDimensions = SelectedRowDimensions;
            DrawBoard();
        }

        /// <summary>
        /// Saves the grid.
        /// </summary>
        private void SaveTileMap()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Rows={0}{1}", SetRowDimensions, Environment.NewLine);
            sb.AppendFormat("Columns={0}{1}", SetColumnDimensions, Environment.NewLine);
            sb.Append("Grid=");
            foreach (var tile in mPlacedTiles)
            {
                sb.AppendFormat("{0},", tile.TileType);
            }
            // Remove trailing comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append(Environment.NewLine);
            var sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.FileName = "map.txt";
            sfd.DefaultExt = ".txt";

            var result = sfd.ShowDialog(this);
            if (result == true)
            {
                // TODO: Save file.
            }
        }

        /// <summary>
        /// Draws the board.
        /// </summary>
        private void DrawBoard()
        {
            for (int i = 0; i < SetRowDimensions; i++)
            {
                for (int j = 0; j < SetColumnDimensions; j++)
                {
                    PlacedTiles.Add(new Tile() { TileType = "0" });
                }
            }
        }

        /// <summary>
        /// Whether or not we can apply settings.
        /// </summary>
        /// <returns><b>True</b> if we can apply settings, <b>False</b> otherwise.</returns>
        private bool CanApplySettings()
        {
            return true;
        }

        /// <summary>
        /// Loads a sprite sheet.
        /// </summary>
        private void LoadSpriteSheet()
        {
            var fod = new OpenFileDialog();
            fod.DefaultExt = ".png";
            fod.Filter = "PNG (.png)|*.png";
            fod.InitialDirectory = Environment.CurrentDirectory;

            var result = fod.ShowDialog(this);
            if (result == true)
            {
                var filePath = fod.FileName;
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    SpriteSheetPath = filePath;

                    Bitmap image = new Bitmap(SpriteSheetPath);
                    System.Windows.Media.Imaging.BitmapImage bmi = new System.Windows.Media.Imaging.BitmapImage();
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        ms.Flush();
                        ms.Position = 0;
                        bmi.BeginInit();
                        bmi.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bmi.StreamSource = ms;
                        bmi.EndInit();
                    }

                    int tileId = 1;
                    for (int row = 0; row < 19; row++)
                    {
                        for (int column = 0; column < 20; column++)
                        {
                            // read 16x16, mmove down to next cell.
                            // Splice rectangle off image.
                            Int32Rect splitRectangle = new Int32Rect(((column * 16) + (1 * column)) + 1, ((row * 16) + (1 * row)) + 1, 16, 16);
                            System.Windows.Media.Imaging.CroppedBitmap cbm = new System.Windows.Media.Imaging.CroppedBitmap(bmi, splitRectangle);

                            // Create tile.
                            Tile tile = new Tile()
                            {
                                Height = 16,
                                Width = 16,
                                CroppedBitmapImage = cbm,
                                TileType = tileId.ToString(),
                            };
                            tileId++;

                            // Add to available tiles (selectable by user).
                            AvailableTiles.Add(tile);
                        }

                    }
                }
            }
        }

        private void PlacedTile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsInEraseMode)
            {
                SelectedPlacedTile.Height = 0;
                SelectedPlacedTile.Width = 0;
                SelectedPlacedTile.TileType = "0";
                SelectedPlacedTile.CroppedBitmapImage = null;
            }
            else if (SelectedAvailableTile != null && SelectedPlacedTile != null)
            {
                SelectedPlacedTile.Height = SelectedAvailableTile.Height;
                SelectedPlacedTile.Width = SelectedAvailableTile.Width;
                SelectedPlacedTile.TileType = SelectedAvailableTile.TileType;
                SelectedPlacedTile.CroppedBitmapImage = SelectedAvailableTile.CroppedBitmapImage;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                IsInEraseMode = !IsInEraseMode;
            }
        }

        private ObservableCollection<Tile> mPlacedTiles;
        private ObservableCollection<Tile> mAvailableTiles;
        private Tile mSelectedAvailableTile;
        private Tile mSelectedPlacedTile;
        private bool mIsInEraseMode;
        private int mScale;
        private string mSpriteSheetPath;
        private int mSetColumnDimensions;
        private int mSetRowDimensions;
        private int mSelectedColumnDimensions;
        private int mSelectedRowDimensions;
        private RelayCommand mApplySettingsCommand;
        private RelayCommand mSaveAsCommand;
        private RelayCommand mLoadSpriteSheetCommand;

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}