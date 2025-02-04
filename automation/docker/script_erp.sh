#!/bin/bash
dotnet /app/control_erp/Control_ERP.dll
echo "Control_ERP Successfully $(TZ='Asia/Bangkok' date +'%F %T')" >> /var/log/cron_erp.log

