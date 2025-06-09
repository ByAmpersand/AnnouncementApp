import React from "react";
import "./Header.css";

const Header = () => {
  return (
    <header className="site-header">
      <div className="logo">
        <span className="logo-text-white">Announce</span>
        <span className="logo-text-orange">
          <span className="hub-box">Hub</span>
        </span>
      </div>
    </header>
  );
};

export default Header;