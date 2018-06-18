using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.BusinessEnum
{
    public enum ErrorCode
    {
        [Description("UserId đã tồn tại, vui lòng chọn user khác.")]
        UserIdExists,

        [Description("UserId không tồn tại.")]
        UserIdNotExists,

        [Description("Mật khẩu không chính xác.")]
        PasswordNotMatch,

        [Description("User không active.")]
        UserNotActive,

        [Description("Lỗi trong hệ thống.")]
        InternalErrorException,

        [Description("Mật khẩu xác nhận không khớp.")]
        VerifyPasswordNotMatch,

        [Description("Mật khẩu cũ không đúng.")]
        OldPasswordNotMatch,

        [Description("User này không được phép truy cập.")]
        NotAllowToLogin,

        [Description("Mã lưu trự đã tồn tại.")]
        StorageIdExists,

        [Description("Mã lưu trữ {0} không tồn tại.")]
        StorageIdNotExists,

        [Description("Mã lưu trữ đã được sử dụng.")]
        StorageIdAlreadyInUse,

        [Description("Mã bệnh phẩm đang tồn tại trong hệ thống.")]
        SampleSpecIdExists,

        [Description("Mã bệnh phẩm không tồn tại trong hệ thống.")]
        SampleSpecIdNotExists,

        [Description("Mã ống xét nghiệm đang tồn tại trong hệ thống.")]
        TubeIdExists,

        [Description("Mã ống xét nghiệm không tồn tại trong hệ thống.")]
        TubeIdNotExists,

        [Description("Mã lưu trữ {0} vị trí {1} đã được sử dụng.")]
        StorageLocationUsed,

        [Description("Mã lưu trữ {0} chỉ tối đa chứa được {1}.")]
        StorageLocationOutOfBound,

        [Description("Không cho phép hủy mẫu trực tiếp, vui lòng xử dụng chức năng hủy.")]
        TubeUpdateStatusRemove,

        [Description("Không cho phép xuất mẫu trực tiếp, vui lòng xử dụng chức năng xuất.")]
        TubeUpdateStatusInUse,

        [Description("Mã xuất đang tồn tại, vui lòng chọn mã khác.")]
        ExportIdExists,

        [Description("Mã xuất không tồn tại.")]
        ExportIdNotExists,

        [Description("Mã nhập không tồn tại.")]
        ImportIdNotExists,

        [Description("Mã hủy đang tồn tại, vui lòng chọn mã khác.")]
        RemovalIdExists,

        [Description("Mã hủy không tồn tại.")]
        RemovalIdNotExists,

        [Description("Vị trí lưu trữ bị trùng.")]
        DuplicateLocationId,

        [Description("Chưa nhập danh sách ống mẫu.")]
        ImportNoTube,

        [Description("Thể tích ống mẫu trống.")]
        ImportTubeEmptyVolume,

        [Description("Dữ liệu trả về nhiều hơn quy định là {0} dòng, vui lòng điều chỉnh lại điều kiện tìm kiếm.")]
        ExceedMaxRowsReturn,

        [Description("Không thể lưu thông tin mẫu đã hủy.")]
        SampleAlreadyRemove,

        [Description("Thể tích mẫu cập nhật vượt quá thể tích hiện tại.")]
        NewVolumeExceedLastVolume,

        [Description("Mã nội dung {0} không tồn tại.")]
        ContentIdNotExists,
    }
}
