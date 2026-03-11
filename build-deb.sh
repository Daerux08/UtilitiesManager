#!/usr/bin/env bash
set -e

APP_NAME="utilitiesmanager"
APP_VERSION="0.4.0~alpha$(date +%Y%m%d%H%M)"
ARCH="amd64"

# project file
PROJECT_PATH="./UtilitiesManager.csproj"

# Output directories
PUBLISH_DIR="./publish"
DEB_DIR="./Package"
DEBIAN_DIR="$DEB_DIR/DEBIAN"
USR_BIN_DIR="$DEB_DIR/usr/bin"
USR_SHARE_DIR="$DEB_DIR/usr/share/UtilitiesManager"

echo "==> Cleaning old builds"
rm -rf "$PUBLISH_DIR"
rm -rf "$DEB_DIR"
rm -f Package.deb
rm -f utilitiesmanager_*.deb

echo "==> Publishing Avalonia app"
dotnet publish "$PROJECT_PATH" -c Release -r linux-x64 --self-contained true -o "$PUBLISH_DIR"

echo "==> Creating Debian package structure"
mkdir -p "$DEBIAN_DIR"
mkdir -p "$USR_BIN_DIR"
mkdir -p "$USR_SHARE_DIR"
mkdir -p "$DEB_DIR/usr/share/applications"

echo "==> Copying published files"
cp -r "$PUBLISH_DIR"/* "$USR_SHARE_DIR"

echo "==> Creating launcher script"
cat <<EOF > "$USR_BIN_DIR/$APP_NAME"
#!/bin/bash
exec /usr/share/UtilitiesManager/UtilitiesManager "\$@"
EOF
chmod +x "$USR_BIN_DIR/$APP_NAME"

echo "==> Creating symlink for common command name"
# Create symlink from UtilMan to utilitiesmanager for easier access
ln -sf "$APP_NAME" "$USR_BIN_DIR/UtilMan"

echo "==> Creating .desktop file"
cat <<EOF > "$DEB_DIR/usr/share/applications/$APP_NAME.desktop"
[Desktop Entry]
Name=Utilities Manager
Exec=$APP_NAME
Icon=utilitiesmanager
Type=Application
Categories=Utility;
EOF

echo "==> Creating control file"
cat <<EOF > "$DEBIAN_DIR/control"
Package: $APP_NAME
Version: $APP_VERSION
Section: utils
Priority: optional
Architecture: $ARCH
Maintainer: Alexander
Description: Utilities Manager - Avalonia application packaged as a .deb
EOF

echo "==> Setting permissions"
chmod -R 755 "$DEB_DIR"

echo "==> Building .deb package"
OUTPUT_FILE="${APP_NAME}_${APP_VERSION}_${ARCH}.deb"
dpkg-deb --build "$DEB_DIR" "$OUTPUT_FILE"

echo "==> Done!"
echo "Created package: $OUTPUT_FILE"
