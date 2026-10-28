namespace GymX.Application.Common.Templates;

public static class EmailTemplates
{
    public static string BuildOtpEmail(string recipientName, string otpCode, string purposeTitle, string purposeDescription)
    {
        return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{purposeTitle}</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; -webkit-font-smoothing: antialiased;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f1f5f9; padding: 40px 15px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 580px; background-color: #ffffff; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 10px 25px rgba(0, 0, 0, 0.05);"">
                    <tr>
                        <td style=""background: linear-gradient(135deg, #011010 0%, #064343 100%); padding: 32px 40px; text-align: center; border-bottom: 3px solid #BAFD00;"">
                            <div style=""display: inline-block;"">
                                <span style=""font-size: 32px; font-weight: 900; letter-spacing: 2px; color: #ffffff;"">GYM</span><span style=""font-size: 32px; font-weight: 900; letter-spacing: 2px; color: #BAFD00;"">X</span>
                            </div>
                            <div style=""color: #71E1E1; font-size: 11px; font-weight: 700; letter-spacing: 2.5px; text-transform: uppercase; margin-top: 6px;"">
                                Premium Fitness Ecosystem
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 40px;"">
                            <h2 style=""margin: 0 0 16px 0; color: #011010; font-size: 22px; font-weight: 800; letter-spacing: -0.5px;"">
                                {purposeTitle}
                            </h2>
                            <p style=""margin: 0 0 12px 0; font-size: 15px; color: #334155; line-height: 1.6;"">
                                Xin chào <b>{recipientName}</b>,
                            </p>
                            <p style=""margin: 0 0 28px 0; font-size: 15px; color: #475569; line-height: 1.6;"">
                                {purposeDescription}
                            </p>
                            <div style=""background-color: #011010; border: 2px solid #BAFD00; border-radius: 14px; padding: 26px 20px; text-align: center; margin-bottom: 28px;"">
                                <div style=""font-size: 11px; font-weight: 700; color: #71E1E1; letter-spacing: 2px; text-transform: uppercase; margin-bottom: 10px;"">
                                    Mã Xác Thực Của Bạn
                                </div>
                                <div style=""font-size: 40px; font-weight: 900; letter-spacing: 12px; color: #BAFD00; font-family: 'SF Pro Display', Consolas, Monaco, monospace; text-shadow: 0 0 15px rgba(186, 253, 0, 0.3); padding-left: 12px;"">
                                    {otpCode}
                                </div>
                                <div style=""display: inline-block; background-color: rgba(186, 253, 0, 0.15); border-radius: 20px; padding: 4px 14px; margin-top: 14px;"">
                                    <span style=""color: #BAFD00; font-size: 12px; font-weight: 700;"">Hiệu lực trong vòng 5 phút</span>
                                </div>
                            </div>
                            <div style=""background-color: #fffbeb; border-left: 4px solid #f59e0b; padding: 14px 16px; border-radius: 6px; margin-bottom: 24px;"">
                                <p style=""margin: 0; font-size: 13px; color: #92400e; line-height: 1.5;"">
                                    <b>Lưu ý an toàn:</b> Tuyệt đối không chia sẻ mã này cho bất kỳ ai, bao gồm cả nhân viên hỗ trợ của GymX, nhằm bảo đảm an toàn cho tài khoản của bạn.
                                </p>
                            </div>
                            <p style=""margin: 0; font-size: 14px; color: #64748b; line-height: 1.6;"">
                                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email hoặc liên hệ với chúng tôi để được trợ giúp.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 24px 40px; border-top: 1px solid #e2e8f0; text-align: center;"">
                            <p style=""margin: 0 0 6px 0; font-size: 12px; color: #64748b; font-weight: 600;"">
                                Đội ngũ Chăm sóc Hội viên GymX
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #94a3b8;"">
                                Hotline: 1900 8888 &bull; Email: support@gymx.vn &bull; Website: gymx.vn
                            </p>
                            <p style=""margin: 12px 0 0 0; font-size: 11px; color: #cbd5e1;"">
                                &copy; 2026 GymX Ecosystem. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string BuildEmployeeWelcomeEmail(string fullName, string employeeCode, string position, string email, string temporaryPassword)
    {
        return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Chào mừng bạn gia nhập GymX</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; -webkit-font-smoothing: antialiased;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f1f5f9; padding: 40px 15px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 580px; background-color: #ffffff; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 10px 25px rgba(0, 0, 0, 0.05);"">
                    <tr>
                        <td style=""background: linear-gradient(135deg, #011010 0%, #064343 100%); padding: 32px 40px; text-align: center; border-bottom: 3px solid #BAFD00;"">
                            <div style=""display: inline-block;"">
                                <span style=""font-size: 32px; font-weight: 900; letter-spacing: 2px; color: #ffffff;"">GYM</span><span style=""font-size: 32px; font-weight: 900; letter-spacing: 2px; color: #BAFD00;"">X</span>
                            </div>
                            <div style=""color: #71E1E1; font-size: 11px; font-weight: 700; letter-spacing: 2.5px; text-transform: uppercase; margin-top: 6px;"">
                                Employee Portal &bull; GymX Ecosystem
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 40px;"">
                            <h2 style=""margin: 0 0 16px 0; color: #011010; font-size: 22px; font-weight: 800; letter-spacing: -0.5px;"">
                                Chào mừng bạn gia nhập GymX!
                            </h2>
                            <p style=""margin: 0 0 12px 0; font-size: 15px; color: #334155; line-height: 1.6;"">
                                Xin chào <b>{fullName}</b>,
                            </p>
                            <p style=""margin: 0 0 24px 0; font-size: 15px; color: #475569; line-height: 1.6;"">
                                Tài khoản nhân sự của bạn đã được kích hoạt thành công trên hệ thống GymX. Dưới đây là thông tin truy cập tài khoản:
                            </p>
                            <div style=""background-color: #011010; border-radius: 14px; padding: 24px; margin-bottom: 24px; border: 1px solid #1e293b;"">
                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                    <tr>
                                        <td style=""padding: 8px 0; color: #71E1E1; font-size: 13px; font-weight: 600; width: 40%;"">Mã nhân viên:</td>
                                        <td style=""padding: 8px 0; color: #BAFD00; font-size: 16px; font-weight: 800; font-family: monospace;"">{employeeCode}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #71E1E1; font-size: 13px; font-weight: 600;"">Vị trí đảm nhiệm:</td>
                                        <td style=""padding: 8px 0; color: #ffffff; font-size: 14px; font-weight: 700;"">{position}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #71E1E1; font-size: 13px; font-weight: 600;"">Email đăng nhập:</td>
                                        <td style=""padding: 8px 0; color: #ffffff; font-size: 14px; font-weight: 600;"">{email}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 8px 0; color: #71E1E1; font-size: 13px; font-weight: 600;"">Mật khẩu tạm thời:</td>
                                        <td style=""padding: 8px 0; color: #BAFD00; font-size: 18px; font-weight: 900; font-family: monospace;"">{temporaryPassword}</td>
                                    </tr>
                                </table>
                            </div>
                            <div style=""background-color: #f0fdf4; border-left: 4px solid #10b981; padding: 14px 16px; border-radius: 6px; margin-bottom: 24px;"">
                                <p style=""margin: 0; font-size: 13px; color: #166534; line-height: 1.5;"">
                                    <b>Khuyến nghị bảo mật:</b> Vui lòng đăng nhập vào cổng thông tin nhân viên và thực hiện đổi mật khẩu ngay trong lần đăng nhập đầu tiên.
                                </p>
                            </div>
                            <p style=""margin: 0; font-size: 14px; color: #64748b; line-height: 1.6;"">
                                Chúc bạn có những trải nghiệm làm việc năng động và tuyệt vời cùng đại gia đình GymX!
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 24px 40px; border-top: 1px solid #e2e8f0; text-align: center;"">
                            <p style=""margin: 0 0 6px 0; font-size: 12px; color: #64748b; font-weight: 600;"">
                                Ban Quản Trị & Nhân Sự GymX
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #94a3b8;"">
                                Hotline: 1900 8888 &bull; Email: hr@gymx.vn &bull; Website: gymx.vn
                            </p>
                            <p style=""margin: 12px 0 0 0; font-size: 11px; color: #cbd5e1;"">
                                &copy; 2026 GymX Ecosystem. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
