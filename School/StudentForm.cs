﻿using SharedModels.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace School
{
    public partial class StudentForm : Form
    {
        private readonly ApiClient _apiClient;
        public StudentForm()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }

        private async void StudentForm_Load(object sender, EventArgs e)
        {
            await LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                var students = await _apiClient.Students.GetAllAsync();
                dgvStudent.DataSource = students.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estudiantes: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            var newStudent = new StudentCreateDto
            {
                Name = txtName.Text,
                Registered = chkRegistered.Checked
            };

            var success = await _apiClient.Students.CreateAsync(newStudent);

            try
            {
                MessageBox.Show("¡Estudiante agregado exitosamente!", "Éxito",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputFields();
                await LoadStudentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar estudiante: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            txtName.Clear();
            chkRegistered.Checked = false;
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var student = (StudentDto)dgvStudent.Rows[e.RowIndex].DataBoundItem;
                txtName.Text = student.Name;
                chkRegistered.Checked = student.Registered;
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if(dgvStudent.SelectedRows.Count > 0)
            {
                var selectedStudent = (StudentDto)dgvStudent.SelectedRows[0].DataBoundItem;
                var updateStudent = new StudentUpdateDto
                {
                    StudentId = selectedStudent.StudentId,
                    Name = txtName.Text,
                    Registered = chkRegistered.Checked,
                };

                try
                {
                    var success = await _apiClient.Students.UpdateAsync(selectedStudent.StudentId,
                updateStudent);

                    if (success)
                    {
                        MessageBox.Show("¡Estudiante actualizado exitosamente!", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                        await LoadStudentsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Error al actualizar estudiante.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar estudiante: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un estudiante para actualizar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
