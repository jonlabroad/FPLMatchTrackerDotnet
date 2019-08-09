import requests
import json
from pprint import pprint

session = requests.session()

url = 'https://users.premierleague.com/accounts/login/'
payload = {
 'password': 'jonl6212',
 'login': 'jon.labroad@gmail.com',
 'redirect_uri': 'https://fantasy.premierleague.com/a/login',
 'app': 'plfpl-web'
}
session.post(url, data=payload)
pprint(vars(session))

response = session.get('https://fantasy.premierleague.com/api/my-team/55385/')
pprint(vars(response))
print(json.loads(response.content))