# api.py

from flask import Flask, request, jsonify, send_file
import cv2
import numpy as np
import io
from PIL import Image

app = Flask(__name__)

@app.route('/recognize', methods=['POST'])
def recognize():
    try:
        # Nhận dữ liệu ảnh từ yêu cầu POST (dưới dạng byte array)
        image_data = request.data
        np_img = np.frombuffer(image_data, dtype=np.uint8)
        image = cv2.imdecode(np_img, cv2.IMREAD_COLOR)

        # Nếu bạn có thêm bước nhận diện nào khác, thực hiện ở đây
        face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, scaleFactor=1.1, minNeighbors=5)

        # Annotate ảnh với khuôn mặt phát hiện được
        for (x, y, w, h) in faces:
            cv2.rectangle(image, (x, y), (x + w, y + h), (255, 0, 0), 2)

        # Chuyển đổi ảnh về định dạng JPEG để gửi lại
        _, buffer = cv2.imencode('.jpg', image)
        image_io = io.BytesIO(buffer)

        response_data = {
            'faces': [{'x': x, 'y': y, 'w': w, 'h': h} for (x, y, w, h) in faces],
            'annotated_image': io.BytesIO(buffer).getvalue()
        }

        return send_file(io.BytesIO(response_data['annotated_image']), mimetype='image/jpeg')
    except Exception as e:
        return jsonify({'status': 'error', 'message': str(e)}), 400

if __name__ == '__main__':
    app.run(debug=True)
