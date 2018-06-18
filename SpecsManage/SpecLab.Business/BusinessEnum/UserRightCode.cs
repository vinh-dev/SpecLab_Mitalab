using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum UserRightCode
    {
        /// <summary>
        /// "Đăng nhập"
        /// </summary>
        [Description("Đăng nhập")]
        R00100 = 100, 

        /// <summary>
        /// Tự thay đổi mật khẩu
        /// </summary>
        [Description("Tự thay đổi mật khẩu")]
        R00101 = 101, 

        /// <summary>
        /// Quản lý user
        /// </summary>
        [Description("Quản lý user")]
        R00200 = 200,

        /// <summary>
        /// Reset mật khẩu user
        /// </summary>
        [Description("Reset mật khẩu user")]
        R00201 = 201, 

        /// <summary>
        /// Quản lý phân quyền
        /// </summary>
        [Description("Quản lý phân quyền")]
        R00202 = 202, 

        /// <summary>
        /// Quản lý lưu trữ
        /// </summary>
        [Description("Quản lý lưu trữ")]
        R00300, 

        /// <summary>
        /// Tạo phiếu hủy
        /// </summary>
        [Description("Truy vấn phiếu hủy")]
        R00400,

        /// <summary>
        /// Tạo phiếu hủy
        /// </summary>
        [Description("Tạo phiếu hủy")]
        R00401, 

        /// <summary>
        /// Tạo phiếu xuất
        /// </summary>
        [Description("Truy vấn phiếu xuất")]
        R00500,

        /// <summary>
        /// Tạo phiếu xuất
        /// </summary>
        [Description("Tạo phiếu xuất")]
        R00501,

        /// <summary>
        /// Tạo phiếu xuất
        /// </summary>
        [Description("Xuất mẫu vượt ngưỡng cảnh báo")]
        R00502, 

        /// <summary>
        /// Quản lý mẫu
        /// </summary>
        [Description("Quản lý mẫu")]
        R00600,

        /// <summary>
        /// Truy vấn lịch sử mẫu
        /// </summary>
        [Description("Truy vấn lịch sử mẫu")]
        R00601,

        /// <summary>
        /// Lưu lại mẫu
        /// </summary>
        [Description("Lưu lại mẫu")]
        R00602,

        /// <summary>
        /// Tái nhập đã hủy
        /// </summary>
        [Description("Ghi đè thông tin mẫu")]
        R00603, 

        /// <summary>
        /// Nhập mẫu vào kho
        /// </summary>
        [Description("Truy vấn phiếu nhập")]
        R00700,

        /// <summary>
        /// Nhập mẫu vào kho
        /// </summary>
        [Description("Nhập mẫu vào kho")]
        R00701, 

        /// <summary>
        /// Thống kê
        /// </summary>
        [Description("Thống kê")]
        R00800,

        /// <summary>
        /// Update thông báo
        /// </summary>
        [Description("Update thông báo")]
        R00900,

    }
}
