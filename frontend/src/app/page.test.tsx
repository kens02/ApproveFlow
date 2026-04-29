import { render, screen } from "@testing-library/react";
import Home from "@/app/page";

describe("Home", () => {
  it("renders dashboard title", () => {
    render(<Home />);
    expect(screen.getByText("ApproveFlow Frontend")).toBeInTheDocument();
  });
});
