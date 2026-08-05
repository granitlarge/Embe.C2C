namespace Embe.C2C.Application;

public static class Emails
{
    public static string Template(string applicationName, string heading, string htmlBody) =>
    $$"""
        <!DOCTYPE html>
        <html lang="en">

        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{{heading}}</title>
        </head>

        <body style="Margin:0;padding:0;background-color:#8F97F0;">

            <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                style="background-color:#8F97F0;">
                <tr>
                    <td align="center" style="padding:32px 12px;">

                        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600"
                            style="width:100%;max-width:600px;">

                            <!-- Main card -->
                            <tr>
                                <td bgcolor="#ffffff" style="
                        padding:24px;
                        font-family:Arial,Helvetica,sans-serif;
                        color:#000000;
                        border-radius:12px;
                    ">

                                    <h1 style="
                            Margin:0 0 24px;
                            font-size:30px;
                            font-weight:bold;
                            text-align:center;
                            line-height:1.3;
                        ">
                                        {{heading}}
                                    </h1>

                                    <div style="
                            font-size:16px;
                            line-height:1.6;
                            color:#333333;
                        ">
                                        {{htmlBody}}
                                    </div>

                                </td>
                            </tr>

                            <!-- Spacer -->
                            <tr>
                                <td height="16"></td>
                            </tr>

                            <!-- Footer -->
                            <tr>
                                <td bgcolor="#ffffff" align="center" style="
                        padding:18px;
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:14px;
                        color:#666666;
                        border-radius:12px;
                    ">
                                    <strong>{{applicationName}} {{DateTime.UtcNow.Year}}</strong>
                                </td>
                            </tr>

                        </table>

                    </td>
                </tr>
            </table>

        </body>

        </html>
    """;

    public static string MatchingCreatedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new match!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see who you matched with!
            </p>
        """
    );

    public static string MessageCreatedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new message!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see the message!
            </p>
        """
    );

    internal static string PositivelyJudgedMessage(string applicationName, string link) => Template
    (
        applicationName,
        "you've got a new like!",
        $$"""
            <p>
                Click <a href='{{link}}'>here</a> to see who liked you!
            </p>
        """
    );

    internal static string VerificationMessage(string applicationName, string code) => Template
    (
        applicationName,
        "verify your e-mail address",
        $$"""
            <p>
                Your e-mail verification code is <strong>{{code}}</strong>.
            </p>
        """
    );
}