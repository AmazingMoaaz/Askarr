/*!

=========================================================
* Argon Dashboard React - v1.0.0
=========================================================

* Product Page: https://www.creative-tim.com/product/argon-dashboard-react
* Copyright 2019 Creative Tim (https://www.creative-tim.com)
* Licensed under MIT (https://github.com/creativetimofficial/argon-dashboard-react/blob/master/LICENSE.md)

* Coded by Creative Tim

=========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*/
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { NavLink as NavLinkRRD, Link } from "react-router-dom";
// nodejs library to set properties for components
import { PropTypes } from "prop-types";
import { getSettings } from "../../store/actions/SettingsActions";
import { getVersionInfo } from "../../store/actions/VersionActions";
import LoadingSpinner from "../Loaders/LoadingSpinner";

// reactstrap components
import {
  Collapse,
  NavbarBrand,
  Navbar,
  NavItem,
  NavLink,
  Nav,
  Container,
  Row,
  Col,
  Badge
} from "reactstrap";



function Sidebar(props) {
  const [collapseOpen, setCollapseOpen] = useState(false);
  const [disableAuthentication, setDisableAuthentication] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const dispatch = useDispatch();
  const versionInfo = useSelector(state => state.version);

  useEffect(() => {
    dispatch(getSettings())
      .then(data => {
        setIsLoading(false);
        setDisableAuthentication(data.payload.disableAuthentication);
      })
      .catch(error => {
        console.error("Error loading settings:", error);
        setIsLoading(false);
      });
    
    // Fetch version info
    dispatch(getVersionInfo());
  }, [dispatch]);



  const routes = props.routes;
  const logo = props.logo;
  let navbarBrandProps;

  if (logo && logo.innerLink) {
    navbarBrandProps = {
      to: logo.innerLink,
      tag: Link
    };
  } else if (logo && logo.outterLink) {
    navbarBrandProps = {
      href: logo.outterLink,
      target: "_blank"
    };
  }

  // toggles collapse between opened and closed (true/false)
  const toggleCollapse = () => {
    setCollapseOpen(!collapseOpen);
  };


  // closes the collapse
  const closeCollapse = () => {
    setCollapseOpen(false);
  };


  // creates the links that appear in the left menu / Sidebar
  const createLinks = (routes) => {
    return routes.filter(r => r.layout !== "/auth").map((prop, key) => {
      return (
        !disableAuthentication || (disableAuthentication && prop.supportsAnonymousUser) ?
          <NavItem key={key}>
            <NavLink
              to={prop.layout + prop.path}
              tag={NavLinkRRD}
              onClick={closeCollapse}
              activeclassname="active"
              className="d-flex align-items-center"
            >
              <i className={prop.icon} />
              <span className="nav-link-text">{prop.name}</span>
              {prop.badge && (
                <Badge color={prop.badgeColor || "primary"} pill className="ml-auto">
                  {prop.badge}
                </Badge>
              )}
            </NavLink>
          </NavItem>
          : null
      );
    });
  };

  if (isLoading) {
    return <LoadingSpinner size={40} />;
  }

  return (
    <Navbar
      className="navbar-vertical fixed-left navbar-light bg-white fade show"
      expand="md"
      id="sidenav-main"
    >
      <Container fluid>
        {/* Toggler */}
        <button
          className="navbar-toggler"
          type="button"
          onClick={toggleCollapse}
        >
          <span className="navbar-toggler-icon" />
        </button>
        {/* Brand */}
        {logo ? (
          <NavbarBrand className="pt-0" {...navbarBrandProps}>
            <img
              style={{ width: '150px' }}
              alt={logo.imgAlt}
              className="navbar-brand-img"
              src={logo.imgSrc}
            />
          </NavbarBrand>
        ) : null}
        {/* Collapse */}
        <Collapse navbar isOpen={collapseOpen}>
          {/* Collapse header */}
          <div className="navbar-collapse-header d-md-none">
            <Row>
              {logo ? (
                <Col className="collapse-brand" xs="6">
                  {logo.innerLink ? (
                    <Link to={logo.innerLink}>
                      <img alt={logo.imgAlt} src={logo.imgSrc} />
                    </Link>
                  ) : (
                    <a href={logo.outterLink}>
                      <img alt={logo.imgAlt} src={logo.imgSrc} />
                    </a>
                  )}
                </Col>
              ) : null}
              <Col className="collapse-close" xs="6">
                <button
                  className="navbar-toggler"
                  type="button"
                  onClick={toggleCollapse}
                >
                  <span />
                  <span />
                </button>
              </Col>
            </Row>
          </div>
          <Nav navbar className="mb-md-3">
            {createLinks(routes)}
          </Nav>
          <hr className="my-3" />
          <h6 className="navbar-heading text-muted">Resources</h6>
          <Nav navbar className="mb-md-3">
            <NavItem>
              <NavLink href={versionInfo.wikiUrl} target="_blank" className="nav-link-icon">
                <i className="fas fa-book mr-2" style={{ color: 'darkgreen' }}></i>
                <span className="nav-link-text">Wiki</span>
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink href={versionInfo.githubUrl} target="_blank" className="nav-link-icon">
                <i className="fab fa-github mr-2" style={{ color: '#000' }}></i>
                <span className="nav-link-text">GitHub</span>
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink href={versionInfo.issuesUrl} target="_blank" className="nav-link-icon">
                <i className="fas fa-bug mr-2 text-danger"></i>
                <span className="nav-link-text">Report Issue</span>
              </NavLink>
            </NavItem>
          </Nav>
          <div className="mt-auto text-center py-4">
            <div className="d-flex flex-column align-items-center">
              {versionInfo.updateAvailable ? (
                <>
                  <small className="text-warning font-weight-bold mb-1">
                    <i className="fas fa-exclamation-circle mr-1"></i>
                    Update Available!
                  </small>
                  <small className="text-muted mb-1">
                    Current: v{versionInfo.currentVersion}
                  </small>
                  <small className="text-success mb-2">
                    Latest: v{versionInfo.latestVersion}
                  </small>
                  <a 
                    href={versionInfo.downloadUrl} 
                    target="_blank" 
                    rel="noopener noreferrer"
                    className="btn btn-sm btn-success"
                    style={{ fontSize: '0.75rem', padding: '0.25rem 0.5rem' }}
                  >
                    <i className="fas fa-download mr-1"></i>
                    Download Update
                  </a>
                </>
              ) : (
                <small className="text-muted">
                  <i className="fas fa-check-circle text-success mr-1"></i>
                  Askarr v{versionInfo.currentVersion}
                </small>
              )}
            </div>
          </div>
        </Collapse>
      </Container>
    </Navbar>
  );
}


Sidebar.defaultProps = {
  routes: [{}]
};

Sidebar.propTypes = {
  // links that will be displayed inside the component
  routes: PropTypes.arrayOf(PropTypes.object),
  logo: PropTypes.shape({
    // innerLink is for links that will direct the user within the app
    // it will be rendered as <Link to="...">...</Link> tag
    innerLink: PropTypes.string,
    // outterLink is for links that will direct the user outside the app
    // it will be rendered as simple <a href="...">...</a> tag
    outterLink: PropTypes.string,
    // the image src of the logo
    imgSrc: PropTypes.string.isRequired,
    // the alt for the img
    imgAlt: PropTypes.string.isRequired
  })
};

export default Sidebar;