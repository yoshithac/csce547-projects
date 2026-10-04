using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WarehouseApp
{
    public class MainForm : Form
    {
        private readonly DataLayer _db = new DataLayer();
        private static readonly string ThemeFile = Path.Combine(AppContext.BaseDirectory, "theme.txt");

        private TabControl tabControl;
        private TabPage tabProducts, tabOrders, tabAdmin, tabSettings;
        private DataGridView gridProducts, gridOrders, gridSuppliers;

        // Products
        private TextBox txtProdId, txtSku, txtProdName, txtCategory, txtPrice, txtStock, txtSearch;
        private ComboBox cboSupplier;
        // Orders
        private TextBox txtOrderId, txtCustomer, txtOrderQty, txtOrderTotal;
        private ComboBox cboOrderProduct;
        // Admin (Suppliers)
        private TextBox txtSuppIdAdmin, txtSuppName, txtSuppEmail, txtSuppPhone, txtSuppCity;
        private CheckBox chkAdminMode;
        private Panel pnlAdminContent;
        // Settings
        private ComboBox cboTheme;

        public MainForm()
        {
            InitializeComponentLayout();
            LoadData();
        }

        // ---------------------------------------------------------------- helpers
        private void Run(Action action)
        {
            try { action(); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Check your input", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("This can't be done because other records still depend on it " +
                    "(for example, a supplier that still has products, or a product that has orders).",
                    "Related records exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            { MessageBox.Show("That value already exists (SKUs must be unique).", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private static bool Confirm(string msg) =>
            MessageBox.Show(msg, "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        private static string Req(TextBox t, string field)
        {
            var v = t.Text.Trim();
            if (v.Length == 0) throw new ArgumentException($"{field} is required.");
            return v;
        }
        private static int ToInt(string s, string field) =>
            int.TryParse(s, out var v) && v >= 0 ? v : throw new ArgumentException($"{field} must be a whole number (0 or more).");
        private static decimal ToDec(string s, string field) =>
            decimal.TryParse(s, out var v) && v >= 0 ? v : throw new ArgumentException($"{field} must be a valid amount (0 or more).");
        private static int SelId(TextBox t) =>
            int.TryParse(t.Text, out var v) ? v : throw new ArgumentException("Select a row in the table first.");
        private static int ComboVal(ComboBox c, string what) =>
            c.SelectedValue is int v ? v : throw new ArgumentException($"Please choose a {what}.");

        // AutoSize so the text is never clipped (the w parameter is kept only so existing calls still compile)
        private static Label Lbl(string text, int x, int y, int w) => new Label { Text = text, Location = new Point(x, y), AutoSize = true };
        private static TextBox Txt(int x, int y, int w, bool readOnly = false) => new TextBox { Location = new Point(x, y), Width = w, ReadOnly = readOnly };
        private Button Btn(string text, int x, int y, int w, Color color, Action click)
        {
            var b = new Button { Text = text, Location = new Point(x, y), BackColor = color, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, MinimumSize = new Size(w, 30) };
            b.Click += (s, e) => Run(click);
            return b;
        }
        private static DataGridView Grid() => new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        private static void ClearAll(params Control[] ctls) { foreach (var c in ctls) c.Text = ""; }

        // ---------------------------------------------------------------- layout
        private void InitializeComponentLayout()
        {
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;   // scales every control with the display's DPI/font size
            Text = "Warehouse Inventory & Order Manager";
            Size = new Size(1020, 740);
            StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabProducts = new TabPage("Products (Search & CRUD)");
            tabOrders = new TabPage("Orders (CRUD)");
            tabAdmin = new TabPage("Administration (Suppliers)");
            tabSettings = new TabPage("Settings");

            SetupProductsTab();
            SetupOrdersTab();
            SetupAdminTab();
            SetupSettingsTab();

            tabControl.TabPages.AddRange(new[] { tabProducts, tabOrders, tabAdmin, tabSettings });
            Controls.Add(tabControl);
            ApplyTheme(this, cboTheme.SelectedIndex == 1);
        }

        // ---------------------------------------------------------------- products
        private void SetupProductsTab()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 140 };

            txtSearch = Txt(225, 12, 250);
            txtSearch.TextChanged += (s, e) => Run(RefreshProducts);
            var btnClear = Btn("Clear Search", 485, 11, 100, SystemColors.Control, () => txtSearch.Clear());

            txtProdId = Txt(45, 47, 50, true);
            txtSku = Txt(150, 47, 90);
            txtProdName = Txt(310, 47, 150);
            txtCategory = Txt(550, 47, 100);
            txtPrice = Txt(60, 82, 70);
            txtStock = Txt(195, 82, 70);
            cboSupplier = new ComboBox
            {
                Location = new Point(345, 82),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "SupplierName",
                ValueMember = "SupplierID"
            };

            var btnAdd = Btn("Insert", 520, 80, 80, Color.LightGreen, () =>
            {
                _db.InsertProduct(Req(txtSku, "SKU"), Req(txtProdName, "Name"), Req(txtCategory, "Category"),
                    ToDec(txtPrice.Text, "Price"), ToInt(txtStock.Text, "Stock"), ComboVal(cboSupplier, "supplier"));
                RefreshProducts(); BindProductLookup();
            });
            var btnUpdate = Btn("Update", 610, 80, 80, Color.LightSkyBlue, () =>
            {
                _db.UpdateProduct(SelId(txtProdId), Req(txtSku, "SKU"), Req(txtProdName, "Name"), Req(txtCategory, "Category"),
                    ToDec(txtPrice.Text, "Price"), ToInt(txtStock.Text, "Stock"), ComboVal(cboSupplier, "supplier"));
                RefreshProducts(); BindProductLookup();
            });
            var btnDelete = Btn("Delete", 700, 80, 80, Color.LightCoral, () =>
            {
                int id = SelId(txtProdId);
                if (!Confirm($"Delete product '{txtProdName.Text}'?")) return;
                _db.DeleteProduct(id);
                RefreshProducts(); BindProductLookup();
            });

            pnlTop.Controls.AddRange(new Control[] {
                Lbl("Search (Name, SKU, Category):", 10, 15, 195), txtSearch, btnClear,
                Lbl("ID:", 10, 50, 30), txtProdId, Lbl("SKU:", 105, 50, 40), txtSku,
                Lbl("Name:", 250, 50, 45), txtProdName, Lbl("Category:", 475, 50, 60), txtCategory,
                Lbl("Price:", 10, 85, 40), txtPrice, Lbl("Stock:", 145, 85, 45), txtStock,
                Lbl("Supplier:", 270, 85, 70), cboSupplier, btnAdd, btnUpdate, btnDelete
            });

            gridProducts = Grid();
            gridProducts.SelectionChanged += (s, e) =>
            {
                if (gridProducts.SelectedRows.Count == 0) { ClearProductFields(); return; }
                var row = gridProducts.SelectedRows[0];
                txtProdId.Text = row.Cells["ProductID"].Value?.ToString();
                txtSku.Text = row.Cells["SKU"].Value?.ToString();
                txtProdName.Text = row.Cells["ProductName"].Value?.ToString();
                txtCategory.Text = row.Cells["Category"].Value?.ToString();
                txtPrice.Text = row.Cells["UnitPrice"].Value?.ToString();
                txtStock.Text = row.Cells["StockQuantity"].Value?.ToString();
                cboSupplier.SelectedValue = row.Cells["SupplierID"].Value;
            };

            tabProducts.Controls.Add(gridProducts);
            tabProducts.Controls.Add(pnlTop);
        }

        private void ClearProductFields() => ClearAll(txtProdId, txtSku, txtProdName, txtCategory, txtPrice, txtStock);

        // Keeps any active search applied after insert/update/delete
        private void RefreshProducts()
        {
            var term = txtSearch.Text.Trim();
            gridProducts.DataSource = term.Length == 0 ? _db.GetAllProducts() : _db.SearchProducts(term);
            if (gridProducts.Rows.Count == 0) ClearProductFields();
        }

        // ---------------------------------------------------------------- orders
        private void SetupOrdersTab()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100 };

            txtOrderId = Txt(75, 17, 50, true);
            txtCustomer = Txt(215, 17, 120);
            cboOrderProduct = new ComboBox
            {
                Location = new Point(405, 17),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Display",
                ValueMember = "ProductID"
            };
            txtOrderQty = Txt(650, 17, 45);
            txtOrderTotal = Txt(760, 17, 80, true);   
            cboOrderProduct.SelectedIndexChanged += (s, e) => RecalcTotal();
            txtOrderQty.TextChanged += (s, e) => RecalcTotal();

            int Qty()
            {
                int q = ToInt(txtOrderQty.Text, "Quantity");
                if (q < 1) throw new ArgumentException("Quantity must be at least 1.");
                return q;
            }

            var btnAdd = Btn("Insert", 100, 55, 90, Color.LightGreen, () =>
            {
                _db.InsertOrder(Req(txtCustomer, "Customer"), ComboVal(cboOrderProduct, "product"), Qty(), ToDec(txtOrderTotal.Text, "Total"));
                RefreshOrders();
            });
            var btnUpdate = Btn("Update", 210, 55, 90, Color.LightSkyBlue, () =>
            {
                _db.UpdateOrder(SelId(txtOrderId), Req(txtCustomer, "Customer"), ComboVal(cboOrderProduct, "product"), Qty(), ToDec(txtOrderTotal.Text, "Total"));
                RefreshOrders();
            });
            var btnDelete = Btn("Delete", 320, 55, 90, Color.LightCoral, () =>
            {
                int id = SelId(txtOrderId);
                if (!Confirm($"Delete order #{id}?")) return;
                _db.DeleteOrder(id);
                RefreshOrders();
            });

            pnlTop.Controls.AddRange(new Control[] {
                Lbl("Order ID:", 10, 20, 60), txtOrderId, Lbl("Customer:", 135, 20, 65), txtCustomer,
                Lbl("Product:", 345, 20, 55), cboOrderProduct, Lbl("Qty:", 615, 20, 35), txtOrderQty,
                Lbl("Total $:", 705, 20, 50), txtOrderTotal, btnAdd, btnUpdate, btnDelete
            });

            gridOrders = Grid();
            gridOrders.SelectionChanged += (s, e) =>
            {
                if (gridOrders.SelectedRows.Count == 0) { ClearAll(txtOrderId, txtCustomer, txtOrderQty, txtOrderTotal); return; }
                var row = gridOrders.SelectedRows[0];
                txtOrderId.Text = row.Cells["OrderID"].Value?.ToString();
                txtCustomer.Text = row.Cells["CustomerName"].Value?.ToString();
                cboOrderProduct.SelectedValue = row.Cells["ProductID"].Value;
                txtOrderQty.Text = row.Cells["Quantity"].Value?.ToString();
                txtOrderTotal.Text = row.Cells["TotalAmount"].Value?.ToString();  // keep stored total
            };

            tabOrders.Controls.Add(gridOrders);
            tabOrders.Controls.Add(pnlTop);
        }

        private void RecalcTotal()
        {
            if (cboOrderProduct.SelectedItem is DataRowView drv && int.TryParse(txtOrderQty.Text, out var q) && q >= 0)
                txtOrderTotal.Text = (Convert.ToDecimal(drv["UnitPrice"]) * q).ToString("F2");
        }

        private void RefreshOrders()
        {
            gridOrders.DataSource = _db.GetAllOrders();
            if (gridOrders.Rows.Count == 0) ClearAll(txtOrderId, txtCustomer, txtOrderQty, txtOrderTotal);
        }

        // ---------------------------------------------------------------- admin (suppliers)
        private void SetupAdminTab()
        {
            var pnlBanner = new Panel { Dock = DockStyle.Top, Height = 45 };
            chkAdminMode = new CheckBox { Text = "Enable Admin Mode (Allow altering Suppliers table)", Location = new Point(15, 12), AutoSize = true };
            chkAdminMode.CheckedChanged += (s, e) => pnlAdminContent.Enabled = chkAdminMode.Checked;
            pnlBanner.Controls.Add(chkAdminMode);

            pnlAdminContent = new Panel { Dock = DockStyle.Fill, Enabled = false };
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 95 };

            txtSuppIdAdmin = Txt(45, 12, 50, true);
            txtSuppName = Txt(215, 12, 150);
            txtSuppEmail = Txt(425, 12, 150);
            txtSuppPhone = Txt(640, 12, 90);
            txtSuppCity = Txt(785, 12, 90);

            var btnAdd = Btn("Insert Supplier", 150, 50, 120, Color.LightGreen, () =>
            {
                _db.InsertSupplier(Req(txtSuppName, "Company name"), Req(txtSuppEmail, "Email"), Req(txtSuppPhone, "Phone"), Req(txtSuppCity, "City"));
                AfterSupplierChange();
            });
            var btnUpdate = Btn("Update Supplier", 290, 50, 120, Color.LightSkyBlue, () =>
            {
                _db.UpdateSupplier(SelId(txtSuppIdAdmin), Req(txtSuppName, "Company name"), Req(txtSuppEmail, "Email"), Req(txtSuppPhone, "Phone"), Req(txtSuppCity, "City"));
                AfterSupplierChange();
            });
            var btnDelete = Btn("Delete Supplier", 430, 50, 120, Color.LightCoral, () =>
            {
                int id = SelId(txtSuppIdAdmin);
                if (!Confirm($"Delete supplier '{txtSuppName.Text}'?\n(Blocked if the supplier still has products.)")) return;
                _db.DeleteSupplier(id);
                AfterSupplierChange();
            });

            pnlTop.Controls.AddRange(new Control[] {
                Lbl("ID:", 10, 15, 25), txtSuppIdAdmin, Lbl("Company:", 110, 15, 95), txtSuppName,
                Lbl("Email:", 380, 15, 45), txtSuppEmail, Lbl("Phone:", 590, 15, 45), txtSuppPhone,
                Lbl("City:", 745, 15, 35), txtSuppCity, btnAdd, btnUpdate, btnDelete
            });

            gridSuppliers = Grid();
            gridSuppliers.SelectionChanged += (s, e) =>
            {
                if (gridSuppliers.SelectedRows.Count == 0) { ClearAll(txtSuppIdAdmin, txtSuppName, txtSuppEmail, txtSuppPhone, txtSuppCity); return; }
                var row = gridSuppliers.SelectedRows[0];
                txtSuppIdAdmin.Text = row.Cells["SupplierID"].Value?.ToString();
                txtSuppName.Text = row.Cells["SupplierName"].Value?.ToString();
                txtSuppEmail.Text = row.Cells["ContactEmail"].Value?.ToString();
                txtSuppPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtSuppCity.Text = row.Cells["City"].Value?.ToString();
            };

            pnlAdminContent.Controls.Add(gridSuppliers);
            pnlAdminContent.Controls.Add(pnlTop);
            tabAdmin.Controls.Add(pnlAdminContent);
            tabAdmin.Controls.Add(pnlBanner);
        }

        private void RefreshSuppliers()
        {
            gridSuppliers.DataSource = _db.GetAllSuppliers();
            if (gridSuppliers.Rows.Count == 0) ClearAll(txtSuppIdAdmin, txtSuppName, txtSuppEmail, txtSuppPhone, txtSuppCity);
        }

        private void AfterSupplierChange()
        {
            RefreshSuppliers();
            BindSupplierLookup();
            RefreshProducts();
        }

        // ---------------------------------------------------------------- settings
        private void SetupSettingsTab()
        {
            cboTheme = new ComboBox { Location = new Point(170, 37), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTheme.Items.AddRange(new[] { "Light Mode", "Dark Mode" });
            bool dark = false;
            try { dark = File.Exists(ThemeFile) && File.ReadAllText(ThemeFile).Trim() == "Dark Mode"; } catch { }
            cboTheme.SelectedIndex = dark ? 1 : 0;
            cboTheme.SelectedIndexChanged += (s, e) =>
            {
                try { File.WriteAllText(ThemeFile, cboTheme.SelectedItem.ToString()); } catch { }
                ApplyTheme(this, cboTheme.SelectedIndex == 1);
            };

            tabSettings.Controls.Add(Lbl("Application Theme:", 30, 40, 130));
            tabSettings.Controls.Add(cboTheme);
        }

        private void ApplyTheme(Control c, bool dark)
        {
            Color bg = dark ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
            Color fg = dark ? Color.White : Color.Black;
            Color input = dark ? Color.FromArgb(30, 30, 30) : Color.White;

            switch (c)
            {
                case DataGridView g:
                    g.BackgroundColor = input;
                    g.DefaultCellStyle.BackColor = input;
                    g.DefaultCellStyle.ForeColor = fg;
                    g.EnableHeadersVisualStyles = !dark;
                    g.ColumnHeadersDefaultCellStyle.BackColor = bg;
                    g.ColumnHeadersDefaultCellStyle.ForeColor = fg;
                    return;
                case TextBox or ComboBox:
                    c.BackColor = input; c.ForeColor = fg;
                    break;
                case Button:
                    c.ForeColor = Color.Black;   // pastel buttons stay readable
                    break;
                default:
                    c.BackColor = bg; c.ForeColor = fg;
                    break;
            }
            foreach (Control child in c.Controls) ApplyTheme(child, dark);
        }

        // ---------------------------------------------------------------- data loading
        private void BindSupplierLookup()
        {
            var cur = cboSupplier.SelectedValue;
            cboSupplier.DataSource = _db.GetSupplierList();
            if (cur != null) cboSupplier.SelectedValue = cur;
        }

        private void BindProductLookup()
        {
            var cur = cboOrderProduct.SelectedValue;
            cboOrderProduct.DataSource = _db.GetProductList();
            if (cur != null) cboOrderProduct.SelectedValue = cur;
        }

        private void LoadData()
        {
            try
            {
                BindSupplierLookup();
                BindProductLookup();
                RefreshProducts();
                RefreshOrders();
                RefreshSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to WarehouseDB: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}