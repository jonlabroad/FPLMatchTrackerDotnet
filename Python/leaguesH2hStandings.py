import sys
import requests
import json
from pprint import pprint

def login():
    session = requests.session()

    login = sys.argv[1]
    password = sys.argv[2]
    
    url = 'https://users.premierleague.com/accounts/login/'
    payload = {
     'password': password,
     'login': login,
     'redirect_uri': 'https://fantasy.premierleague.com/a/login',
     'app': 'plfpl-web'
    }
    session.post(url, data=payload)
    return session

session = login()

leagueId = sys.argv[3]
page = sys.argv[4]

response = session.get(f'https://fantasy.premierleague.com/api/leagues-h2h/{leagueId}/standings/?page_new_entries=1&page_standings={page}')
sys.stdout.write(json.dumps(json.loads(response.content)))
