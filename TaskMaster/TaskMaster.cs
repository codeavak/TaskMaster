using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskMaster.Models;

namespace TaskMaster
{
    public partial class TaskMaster : Form
    {
        private tmDbContext dbContext;

        public TaskMaster()
        {
            InitializeComponent();
            dbContext = new tmDbContext();
            var statuses = dbContext.Statuses.ToList();
            foreach(var s in statuses)
            {
                cbStatuses.Items.Add(s);
            }

            RefreshData();


        }

        private void RefreshData()
        {
            BindingSource b1 = new BindingSource();
            var dataSource = from t in dbContext.Tasks
                             orderby t.DueDate
                             select new { t.Id, Task = t.Name, Status = t.Status.Name, t.DueDate };
            b1.DataSource = dataSource.ToList();
            dgvTasks.DataSource = b1;
            dgvTasks.Refresh();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTask.Text))
            {
                MessageBox.Show("Please enter task before clicking Create button!");
                return;
            }
            if (cbStatuses.SelectedItem == null)
            {
                MessageBox.Show("Please select a status for the task!");
                return;
            }
            var dbContext = new tmDbContext();
            var status = (cbStatuses.SelectedItem as Status);
            var newTask = new Models.Task { Name = txtTask.Text, DueDate = dtDueDate.Value, StatusId=status.Id };
            dbContext.Tasks.Add(newTask);
            dbContext.SaveChanges();
            RefreshData();
            txtTask.Text = "";
        }

        private void dgvTasks_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var dataGridView = dgvTasks as DataGridView;
            if (dataGridView != null)
            {
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.Columns[dataGridView.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Please select a row before clicking delete"); return;
            }
            var t = dbContext.Tasks.Find(dgvTasks.SelectedCells[0].Value);
            dbContext.Tasks.Remove(t);
            dbContext.SaveChanges();
            RefreshData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count<=0)
            {
                MessageBox.Show("Please select a row before clicking update");return;
            }
            if (btnUpdate.Text == "Update")
            {
                btnUpdate.Text = "Save";
                var t = dbContext.Tasks.Find(dgvTasks.SelectedCells[0].Value);
                txtTask.Text = t.Name;
                dtDueDate.Value = (DateTime)t.DueDate;
                cbStatuses.SelectedItem = t.Status;
            }

            else
            {
                var t = dbContext.Tasks.Find(dgvTasks.SelectedCells[0].Value);
                t.Name = txtTask.Text;
                t.DueDate = dtDueDate.Value;
                var status = cbStatuses.SelectedItem as Status;
                if (status == null)
                {
                    MessageBox.Show("Please select a status");return;
                }
                if (string.IsNullOrEmpty(txtTask.Text))
                {
                    MessageBox.Show("Please enter a name for the task");return;
                }

                t.StatusId = status.Id;
                dbContext.SaveChanges();
                RefreshData();
                
                btnUpdate.Text = "Update";
                txtTask.Text = "";
                dtDueDate.Value = DateTime.Now;
                cbStatuses.SelectedItem = null;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnUpdate.Text = "Update";
            txtTask.Text = "";
            dtDueDate.Value = DateTime.Now;
            cbStatuses.SelectedItem = null;
        }

        private void dgvTasks_SelectionChanged(object sender, EventArgs e)
        {
            btnUpdate.Text = "Update";
            txtTask.Text = "";
            dtDueDate.Value = DateTime.Now;
            cbStatuses.SelectedItem = null;
        }
    }
}
