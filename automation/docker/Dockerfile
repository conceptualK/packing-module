# Use an official base image, like Ubuntu
FROM ubuntu:latest

# Install .NET 8.0
RUN apt-get update && \
    apt-get install -y wget apt-transport-https && \
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0

# Install necessary dependencies
RUN apt-get install -y wget curl && \
    apt-get install -y tzdata

# Set environment variables to set the time zone (optional, if required)
ENV TZ=Asia/Bangkok
RUN dpkg-reconfigure --frontend noninteractive tzdata

# Install cron and other dependencies
RUN apt-get install -y cron

# Set the working directory
WORKDIR /app

# Copy the shell scripts into the container
COPY script_erp.sh /app/script_erp.sh
COPY script_part.sh /app/script_part.sh

COPY ./UpdatePart /app/update_part
COPY ./ControlERP /app/control_erp

# Make the shell scripts executable
RUN chmod +x /app/script_erp.sh
RUN chmod +x /app/script_part.sh

# Create the cron log file and set the right permissions
RUN touch /var/log/cron_part.log && chmod 0664 /var/log/cron_part.log
RUN touch /var/log/cron_erp.log && chmod 0664 /var/log/cron_erp.log

# Add crontab entries for running the scripts every minute and every 2 minutes
RUN echo "00 8 * * * root /app/script_part.sh >> /var/log/cron_part.log 2>&1" >> /etc/cron.d/mycron
RUN echo "* */2 * * * root /app/script_erp.sh >> /var/log/cron_erp.log 2>&1" >> /etc/cron.d/mycron

# Apply cron job and set proper permissions
RUN chmod 0644 /etc/cron.d/mycron

# Start cron and run it in the foreground
CMD cron -f
