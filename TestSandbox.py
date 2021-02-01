import base64
import hmac
import hashlib

from datetime import datetime
from uuid import uuid4

import requests


API_KEY = '<YOUR API KEY>'
API_SECRET = '<YOUR API SECRET>'.encode('utf-8')


# Get profiles
nonce = str(uuid4())
now = datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ')

headers = {
    'PaymentService-Date': now,
    'PaymentService-Nonce': nonce,
}
method = 'GET'
path = '/v1/profiles'

signature = f'{method}\n{path}\n\npaymentservice-contenthash:\npaymentservice-date:{headers["PaymentService-Date"]}\npaymentservice-nonce:{headers["PaymentService-Nonce"]}'
hmac_hash = hmac.new(API_SECRET, signature.encode('utf-8'), hashlib.sha256)
b64 = base64.b64encode(hmac_hash.digest()).decode('utf-8')
headers['Authorization'] = f'Signature {API_KEY}:{b64}'

response = requests.get(
    "https://sandbox-api.gocustomate.com/v1/profiles",
    headers=headers,
)
