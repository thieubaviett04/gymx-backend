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
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
    <style>
        body, table, td, p, h2, div, span {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif !important;
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; -webkit-font-smoothing: antialiased;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f8fafc; padding: 40px 16px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 520px; background-color: #ffffff; border-radius: 12px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);"">
                    <tr>
                        <td style=""padding: 36px 36px 12px 36px; text-align: center;"">
                            <div style=""display: inline-block;"">
                                <span style=""font-size: 26px; font-weight: 900; letter-spacing: -0.5px; color: #064343;"">Gym</span><span style=""font-size: 26px; font-weight: 900; letter-spacing: -0.5px; color: #7AA900;"">Fitness</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 12px 36px 36px 36px;"">
                            <h2 style=""margin: 0 0 16px 0; color: #0f172a; font-size: 20px; font-weight: 700; letter-spacing: -0.3px;"">
                                {purposeTitle}
                            </h2>
                            <p style=""margin: 0 0 12px 0; font-size: 14px; color: #334155; line-height: 1.6;"">
                                Xin chào <b>{recipientName}</b>,
                            </p>
                            <p style=""margin: 0 0 24px 0; font-size: 14px; color: #475569; line-height: 1.6;"">
                                {purposeDescription}
                            </p>
                            <div style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 22px 16px; text-align: center; margin: 24px 0;"">
                                <div style=""font-size: 11px; font-weight: 600; color: #64748b; letter-spacing: 1.5px; text-transform: uppercase; margin-bottom: 8px;"">
                                    Mã xác thực OTP
                                </div>
                                <div style=""font-size: 36px; font-weight: 800; letter-spacing: 8px; color: #064343; font-family: 'Inter', Consolas, Monaco, monospace; padding-left: 8px;"">
                                    {otpCode}
                                </div>
                                <div style=""font-size: 12px; color: #64748b; margin-top: 8px;"">
                                    Mã có hiệu lực trong vòng <b>5 phút</b>
                                </div>
                            </div>
                            <p style=""margin: 0 0 16px 0; font-size: 13px; color: #64748b; line-height: 1.6;"">
                                Để bảo mật tài khoản, vui lòng không chia sẻ mã này cho bất kỳ ai. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 20px 36px; border-top: 1px solid #e2e8f0; text-align: center;"">
                            <p style=""margin: 0 0 4px 0; font-size: 12px; color: #64748b;"">
                                Đội ngũ Hỗ trợ GymFitness &bull; Hotline: 1900 8888
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #94a3b8;"">
                                &copy; 2026 GymFitness. All rights reserved.
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
    <title>Chào mừng bạn gia nhập GymFitness</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
    <style>
        body, table, td, p, h2, div, span {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif !important;
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; -webkit-font-smoothing: antialiased;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f8fafc; padding: 40px 16px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 520px; background-color: #ffffff; border-radius: 12px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);"">
                    <tr>
                        <td style=""padding: 36px 36px 12px 36px; text-align: center;"">
                            <div style=""display: inline-block;"">
                                <span style=""font-size: 26px; font-weight: 900; letter-spacing: -0.5px; color: #064343;"">Gym</span><span style=""font-size: 26px; font-weight: 900; letter-spacing: -0.5px; color: #7AA900;"">Fitness</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 12px 36px 36px 36px;"">
                            <h2 style=""margin: 0 0 16px 0; color: #0f172a; font-size: 20px; font-weight: 700; letter-spacing: -0.3px;"">
                                Chào mừng bạn gia nhập GymFitness!
                            </h2>
                            <p style=""margin: 0 0 12px 0; font-size: 14px; color: #334155; line-height: 1.6;"">
                                Xin chào <b>{fullName}</b>,
                            </p>
                            <p style=""margin: 0 0 20px 0; font-size: 14px; color: #475569; line-height: 1.6;"">
                                Tài khoản nhân sự của bạn đã được khởi tạo thành công trên hệ thống. Dưới đây là thông tin truy cập:
                            </p>
                            <div style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 18px 20px; margin-bottom: 20px;"">
                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                    <tr>
                                        <td style=""padding: 6px 0; color: #64748b; font-size: 13px; width: 40%;"">Mã nhân viên:</td>
                                        <td style=""padding: 6px 0; color: #064343; font-size: 14px; font-weight: 700; font-family: 'Inter', monospace;"">{employeeCode}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 6px 0; color: #64748b; font-size: 13px;"">Vị trí đảm nhiệm:</td>
                                        <td style=""padding: 6px 0; color: #0f172a; font-size: 14px; font-weight: 600;"">{position}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 6px 0; color: #64748b; font-size: 13px;"">Email đăng nhập:</td>
                                        <td style=""padding: 6px 0; color: #0f172a; font-size: 14px; font-weight: 600;"">{email}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 6px 0; color: #64748b; font-size: 13px;"">Mật khẩu tạm thời:</td>
                                        <td style=""padding: 6px 0; color: #064343; font-size: 16px; font-weight: 800; font-family: 'Inter', monospace;"">{temporaryPassword}</td>
                                    </tr>
                                </table>
                            </div>
                            <p style=""margin: 0; font-size: 13px; color: #64748b; line-height: 1.6;"">
                                Vui lòng đăng nhập và tiến hành đổi mật khẩu ngay trong lần sử dụng đầu tiên để đảm bảo tính an toàn.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 20px 36px; border-top: 1px solid #e2e8f0; text-align: center;"">
                            <p style=""margin: 0 0 4px 0; font-size: 12px; color: #64748b;"">
                                Ban Quản Trị & Nhân Sự GymFitness
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #94a3b8;"">
                                &copy; 2026 GymFitness. All rights reserved.
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
