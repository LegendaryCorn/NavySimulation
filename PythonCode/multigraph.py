import matplotlib.pyplot as plt
import numpy as np
import os
import sys

def main():

    if(len(sys.argv) != 2 or not os.path.exists(sys.argv[1])):
        print("Invalid input.")
        return
    

    file_list = os.listdir(sys.argv[1])

    avg_all = []
    max_all = []

    for file_name in file_list:
        file_path = sys.argv[1] + "/" + file_name
        file = open(file_path, "r")

        avg = []
        max = []
        
        l = file.readline()
        while l:
            sl = l.split(" ")
            # Add to avg
            avg.append(float(sl[2].strip(",\n")))
            # Add to max
            max.append(float(sl[3].strip(",\n")))

            l = file.readline()
            l = file.readline()
        
        avg_all.append(avg)
        max_all.append(max)
    
    np_avg_all = np.array(avg_all)
    np_max_all = np.array(max_all)

    plt.plot(range(np_avg_all.shape[1]), np.average(np_avg_all, 0), label = "Average")
    plt.plot(range(np_max_all.shape[1]), np.average(np_max_all, 0), label = "Maximum")
    plt.legend()
    plt.show()

if __name__ == "__main__":
    main()