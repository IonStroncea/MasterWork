

import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

def analyze_csv_data(df, column):
    # Check if the required columns exist
    
    # Extract data column
    data = df[column]
    
    # Calculate average
    average = data.mean()
    print(f"Average: {average}")
    
    # Calculate median
    median = data.median()
    print(f"Median: {median}")
    
    # Calculate distribution
    distribution = data.value_counts().sort_index()
    
    std_dev = data.std()
    print(f"Standard Deviation: {std_dev}")

    # Calculate percentiles
    percentiles = np.percentile(data, [25, 50, 75])
    print(f"25th Percentile: {percentiles[0]}")
    print(f"50th Percentile: {percentiles[1]}")
    print(f"75th Percentile: {percentiles[2]}")
    
    # Plotting
    plt.figure(figsize=(15, 10))
    
    # Plot distribution
    plt.subplot(2, 2, 1)
    plt.hist(data, bins=20, color='skyblue', edgecolor='black')
    plt.title('Data Distribution')
    plt.xlabel('Data Values')
    plt.ylabel('Frequency')
    
    # Plot time series
    plt.subplot(2, 2, 2)
    plt.plot(df['datetime'], data, color='green')
    plt.title('Data Over Time')
    plt.xlabel('DateTime')
    plt.ylabel('Data')
    
    # Box plot for percentiles
    plt.subplot(2, 2, 3)
    plt.boxplot(data, vert=False)
    plt.title('Box Plot of Data')
    plt.xlabel('Data Values')
    
    # Summary statistics bar chart
    plt.subplot(2, 2, 4)
    plt.axis('off')
    summary_text = (
        f"Average: {average}\n"
        f"Median: {median}\n"
        f"Standard Deviation: {std_dev}\n"
        f"25th Percentile: {percentiles[0]}\n"
        f"50th Percentile: {percentiles[1]}\n"
        f"75th Percentile: {percentiles[2]}"
    )
    plt.text(0.1, 0.5, summary_text, fontsize=12, va='center')
    plt.title('Summary Statistics')
    
    plt.tight_layout()
    plt.show()

def plot_wait_time(sender_file='Sender', server_file='Server', fileName="Simple"):
    output_file = sender_file + fileName+".png"

    sender_file = sender_file + ".csv"

    server_file = server_file + ".csv"

    sender_df = pd.read_csv(sender_file, names=['datetime', 'amount', 'message'])
    server_df = pd.read_csv(server_file, names=['datetime', 'amount'])

    # Convert datetime columns to datetime objects
    sender_df['datetime'] = pd.to_datetime(sender_df['datetime'], format='%Y-%m-%d %H:%M:%S.%f')
    server_df['datetime'] = pd.to_datetime(server_df['datetime'], format='%Y-%m-%d %H:%M:%S.%f')

    i, j = 0, 0
    wait_times = []
    timestamps = []
    sendT = 0
    servT = 0
    valueStamps = []
    # step = int(sender_df.at[0, 'amount'] / server_df.at[0, 'amount']) + 1
    # j = step - 1
    # print(step)
    step = 1
    vT=0

    while i < len(sender_df) and j < len(server_df):
        # sendT += sender_df.at[i, 'amount']
        # servT += server_df.at[j, 'amount']
        # if sendT == servT:
        wait_time = (server_df.at[j, 'datetime'] - sender_df.at[i, 'datetime']).total_seconds()
        #print("" + str(server_df.at[j, 'datetime']) +  "-" + str(sender_df.at[j, 'datetime']) + "=" + str(wait_time) + "---" + str((server_df.at[j, 'datetime'] - sender_df.at[i, 'datetime'])))
        wait_times.append(wait_time)
        timestamps.append(sender_df.at[i, 'datetime'])
        valueStamps.append(vT)
        i += 1
        j += step
        vT+= step
        # elif sender_df.at[i, 'datetime'] < server_df.at[j, 'datetime']:
        #     i += 1
        #     servT -= server_df.at[j, 'amount']
        # else:
        #     j += 1
        #     sendT -= sender_df.at[i, 'amount']

    # Plotting the wait time
    plt.figure(figsize=(10, 6))
    plt.plot(valueStamps, wait_times, marker='.', linestyle='-', color='r')
    plt.xlabel('Timestamp (Sender)')
    plt.ylabel('Wait Time (seconds)')
    plt.title('Wait Time Between Sending and Receiving Data')
    plt.xticks(rotation=45)
    plt.tight_layout()
    plt.savefig(output_file)
    plt.show()
    plt.close()
    df = pd.DataFrame({'datetime': valueStamps, 'data': wait_times})
    analyze_csv_data(df, 'data')

def plot_system_usage(csv_file='Values.csv', name = "Simple"):
    ram_output_file = "ram_" + name + ".png"
    cpu_output_file = "cpu_" + name + ".png"
    # Load SystemUsage.csv
    usage_df = pd.read_csv(csv_file, names=['datetime', 'ram_mb', 'cpu_percent'])

    # Convert datetime column to datetime objects
    usage_df['datetime'] = pd.to_datetime(usage_df['datetime'], format='%Y-%m-%d %H:%M:%S.%f')   

    # Plotting RAM usage
    plt.figure(figsize=(10, 6))
    plt.plot(usage_df['datetime'], usage_df['ram_mb'], marker='.', linestyle='-', color='g')
    plt.xlabel('Timestamp')
    plt.ylabel('RAM Usage (MB)')
    plt.title('RAM Usage Over Time')
    plt.xticks(rotation=45)
    plt.tight_layout()
    plt.savefig(ram_output_file)
    plt.show()
    plt.close()

    usage_df = usage_df[usage_df['cpu_percent'] != 0]

    # Plotting CPU usage
    plt.figure(figsize=(10, 6))
    plt.plot(usage_df['datetime'], usage_df['cpu_percent'], marker='.', linestyle='-', color='r')
    plt.xlabel('Timestamp')
    plt.ylabel('CPU Usage (%)')
    plt.title('CPU Usage Over Time')
    plt.xticks(rotation=45)
    plt.tight_layout()
    plt.savefig(cpu_output_file)
    plt.show()
    plt.close()

    analyze_csv_data(usage_df, 'ram_mb')
    analyze_csv_data(usage_df, 'cpu_percent')