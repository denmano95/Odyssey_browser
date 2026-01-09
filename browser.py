import sys
import xml.etree.ElementTree as ET
from PyQt6.QtCore import QUrl
from PyQt6.QtWidgets import QApplication, QMainWindow, QToolBar, QLineEdit, QVBoxLayout, QWidget, QLabel
from PyQt6.QtWebEngineCore import QWebEngineProfile
from PyQt6.QtWebEngineWidgets import QWebEngineView
from PyQt6.QtGui import QAction, QIcon, QPixmap, QMouseEvent
from PyQt6.QtCore import pyqtSignal, Qt

class ClickableImageLabel(QLabel):
    clicked = pyqtSignal()

    def mousePressEvent(self, event: QMouseEvent):
        self.clicked.emit()
        super().mousePressEvent(event)

class SimpleBrowser(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Simple Python Browser")
        self.setGeometry(100, 100, 1024, 768)

        # Load Configuration
        self.config = self.load_config()
        
        # Central widget
        self.central_widget = QWidget()
        self.setCentralWidget(self.central_widget)
        self.layout = QVBoxLayout(self.central_widget)
        self.layout.setContentsMargins(0, 0, 0, 0)

        # Web Engine View
        # Handle Cookies Setting BEFORE creating the view if possible, or on the profile used by the view
        # But QWebEngineView uses the default profile by default.
        self.browser = QWebEngineView()
        
        if self.config.get("ignore_cookies", "false") == "true":
             # Use a cookie filter to reject all cookies
             profile = QWebEngineProfile.defaultProfile()
             profile.setPersistentCookiesPolicy(QWebEngineProfile.PersistentCookiesPolicy.NoPersistentCookies)
             profile.cookieStore().setCookieFilter(lambda request: False)

        startup_url = self.config.get("startup_url", "https://www.google.com")
        self.browser.setUrl(QUrl(startup_url))
        self.layout.addWidget(self.browser)

        # Navigation Bar
        self.navbar = QToolBar()
        self.addToolBar(self.navbar)

        # Back Button
        back_btn = QAction("Back", self)
        back_btn.setStatusTip("Back to previous page")
        back_btn.triggered.connect(self.browser.back)
        self.navbar.addAction(back_btn)

        # Forward Button
        forward_btn = QAction("Forward", self)
        forward_btn.setStatusTip("Forward to next page")
        forward_btn.triggered.connect(self.browser.forward)
        self.navbar.addAction(forward_btn)

        # Reload Button
        reload_btn = QAction("Reload", self)
        reload_btn.setStatusTip("Reload page")
        reload_btn.triggered.connect(self.browser.reload)
        self.navbar.addAction(reload_btn)

        # Address Bar
        self.url_bar = QLineEdit()
        self.url_bar.returnPressed.connect(self.navigate_to_url)
        self.navbar.addWidget(self.url_bar)

        # Update URL bar when page changes
        self.browser.urlChanged.connect(self.update_url)

        # Overlay Close Button (farmo.png)
        self.close_btn = ClickableImageLabel(self)
        self.close_btn.setPixmap(QPixmap("farmo.png"))
        self.close_btn.setScaledContents(True) # In case we want to force size, but let's stick to original or handle resize if needed
        # Assuming original size or reasonable size. If specific size needed, we can setFixedSize.
        # User didn't specify size, just position.
        self.close_btn.clicked.connect(self.close)
        self.close_btn.hide()

        # Handle Fullscreen
        if self.config.get("fullscreen", "false") == "true":
            self.showFullScreen()
            self.navbar.hide()
            self.close_btn.show()
            self.close_btn.raise_()
            self.update_overlay_position()
        else:
            self.show()

    def resizeEvent(self, event):
        self.update_overlay_position()
        super().resizeEvent(event)

    def update_overlay_position(self):
        if self.config.get("fullscreen", "false") == "true" and self.close_btn.isVisible():
             # Size: specified w,h or default
             size_str = self.config.get("button_size", "")
             if size_str:
                 try:
                     parts = size_str.split(',')
                     if len(parts) == 2:
                         w = int(parts[0].strip())
                         h = int(parts[1].strip())
                         self.close_btn.resize(w, h)
                 except ValueError:
                     pass

             # Position: specified offset from top-left, or default top-right logic
             offset_str = self.config.get("button_offset", "")
             clicked_move = False
             if offset_str:
                 try:
                     parts = offset_str.split(',')
                     if len(parts) == 2:
                         x = int(parts[0].strip())
                         y = int(parts[1].strip())
                         self.close_btn.move(x, y)
                         clicked_move = True
                 except ValueError:
                     pass # Fallback to default
             
             if not clicked_move:
                 # Default fallback: top right, 50px from top and right
                 pm = self.close_btn.pixmap()
                 if pm:
                     w = pm.width() if self.close_btn.width() == 0 else self.close_btn.width() # Use current width if resized? Or original?
                     # If we resized the label, uses label width. If not, maybe label width is 0 or implicit?
                     # If setScaledContents is true, label size dictates image size.
                     # If no size config, label likely takes pixmap size by default or previous size.
                     # Let's rely on self.close_btn.width() which should be correct if resized or default.
                     w = self.close_btn.width() 
                     self.close_btn.move(self.width() - w - 50, 50)

    def load_config(self):
        config = {
            "fullscreen": "false",
            "startup_url": "https://www.google.com",
            "ignore_cookies": "false",
            "button_offset": "",
            "button_size": ""
        }
        try:
            tree = ET.parse('config.xml')
            root = tree.getroot()
            for child in root:
                config[child.tag] = child.text
        except Exception as e:
            print(f"Error loading config: {e}")
        return config

    def navigate_to_url(self):
        url = self.url_bar.text()
        if not url.startswith("http"):
            url = "http://" + url
        self.browser.setUrl(QUrl(url))

    def update_url(self, q):
        self.url_bar.setText(q.toString())

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = SimpleBrowser()
    # window.show() is called inside __init__ based on config
    sys.exit(app.exec())
