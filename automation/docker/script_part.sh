#!/bin/bash
dotnet /app/update_part/Control_Part.dll
echo "Control_Part Successfully $(TZ='Asia/Bangkok' date +'%F %T')" >> /var/log/cron_part.log

