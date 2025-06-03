import React from "react";
import { Oval } from "react-loader-spinner";
import { Container, Row, Col } from "reactstrap";

const LoadingSpinner = ({ size = 80, color = "#5e72e4" }) => {
  return (
    <Container className="py-5 d-flex align-items-center justify-content-center" style={{ minHeight: "70vh" }}>
      <Row>
        <Col className="text-center">
          <Oval
            height={size}
            width={size}
            color={color}
            wrapperStyle={{}}
            wrapperClass=""
            visible={true}
            ariaLabel="loading"
            secondaryColor="#ccc"
            strokeWidth={4}
            strokeWidthSecondary={4}
          />
          <h4 className="mt-3 text-muted">Loading...</h4>
        </Col>
      </Row>
    </Container>
  );
};

export default LoadingSpinner; 