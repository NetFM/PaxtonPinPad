"""
A script to call api on visitor.express for latest
"""
import requests
import datetime
import json
import sys
import os
from datetime import timedelta, date

import subprocess

pindir = os.path.normpath("C://Users/dave/ve/AddUser/bin/Debug/")

os.chdir(pindir)

dd = os.getcwd()

logfile = open('logile', 'a')

logfile.write('Start new run...\n')
logfile.write(dd + 'dir...\n')

host = "https://visitor.express/"
#host = "http://localhost:8000/"
#host = "https://visitor.express.netfm.org/"
#host = "https://visitor.express.netfm.org/"
token = "Token 0c7fb03fa775f0313364967ab0bb8255650cfd0c"
carpark = "Visitors carpark (Car Park 2)"
site = "Vodafone HQ"

def get_latest(start, end):

    start_str = start.strftime('%Y-%m-%d')
    end_str = end.strftime('%Y-%m-%d')
    dep_str = "Visitors_" + start.strftime('%m_%d')
  

    #print (start_str, end_str)
    logfile.write(start_str + end_str + '...\n')


    payload = {'carpark': carpark, 'site': site, 'start': start_str, 'end': end_str}
    r = requests.post(host + "barrier/allowed_pins/", headers={"Authorization": token},
                      data=payload)
    assert r.status_code == 200
#    print(r.text)
    data = json.loads(r.text)


    for record in data:
        print(record)
        print('\n')
        user = record['first_name'] + ' ' + record['second_name'] + ' dummy_email@company.com ' + record['start'] + ' ' + record[
'end'] + ' ' + str(format(record['pin_code'],"04"))
        #print(format(record['pin_code'],"04"))  # To deal with zeros at front of 4 digit pins
        cmd = 'PinUser.exe adduser' + ' ' + dep_str + ' ' + user
        logfile.write(cmd + '\n')
        subprocess.run(cmd)

		
if __name__ == "__main__":
    start_date = date(2013, 1, 1)
    end_date = date(2015, 6, 2)

    start_date = datetime.datetime.now()

    end_date = start_date + timedelta(days=3)
    #start_date = start_date + timedelta(days=6)

    
    delta = end_date - start_date

    for d in range(delta.days + 1):
        start = start_date + timedelta(days=d)
        end = start + timedelta(days=1)
        #print( start, end)
        get_latest(start, end)

		

		

		