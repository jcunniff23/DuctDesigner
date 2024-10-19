import marimo

__generated_with = "0.9.1"
app = marimo.App(width="medium")


@app.cell
def __():
    import marimo as mo
    import plotly.express as px
    import plotly.graph_objs as go
    import pandas as pd
    import numpy as np
    return go, mo, np, pd, px


@app.cell
def __(pd):
    d = {'x':[]}
    terminal_locations = pd.DataFrame(data=d)

    duct_mains = []
    return d, duct_mains, terminal_locations


@app.cell
def __(go):
    fig_lines = go.Scatter3d(
        x=[0,1,None, 0,1,None],
        y=[0,1,None, 0,0,None],
        z=[0,0,None, 0,0,None],
        mode='lines',
        line=dict(color='rgb(125,125,125)', width=1),
        hoverinfo='none'    
    )
    return (fig_lines,)


@app.cell
def __(go):
    fig_nodes = go.Scatter3d(
                            x=[0,1,0,1],
                            y=[0,1,1,0],
                            z=[0,0,0,0],
                            mode='markers',
                            marker=dict(symbol='circle',
                                        size=6,
                                        colorscale='Viridis',
                                        line=dict(color='rgb(50,50,50)', width=0.5)),
                            )
    return (fig_nodes,)


@app.cell
def __(fig_lines, fig_nodes, go, mo):
    axis=dict(showbackground=False,
              showline=False,
              zeroline=False,
              showgrid=False,
              showticklabels=False,
              title=''
              )
    fig = go.Figure(data=[fig_lines, fig_nodes])
    fig.update_layout(scene=dict(xaxis = dict(axis),
                                 yaxis = dict(axis),
                                 zaxis = dict(axis)))
    plot = mo.ui.plotly(
        fig

    )
    return axis, fig, plot


@app.cell
def __(mo, plot):
    mo.hstack([plot, plot.value])
    return


@app.cell
def __():
    from gpt_solver import SimulatedAnnealing
    return (SimulatedAnnealing,)


@app.cell
def __(SimulatedAnnealing):
    SimulatedAnnealing()
    return


@app.cell
def __():
    {
        3: (12, 5),
        4: (15, 10),
        5: (8, 12)
    }
    return


@app.cell
def __(SimulatedAnnealing):
    terminal_locations = {
        3: (12, 5),
        4: (15, 10),
        5: (8, 12)
    }

    # Create an instance of SimulatedAnnealing with terminal locations
    sa = SimulatedAnnealing({
        3: (12, 5),
        4: (15, 10),
        5: (8, 12)
    })

    # Run the simulated annealing process
    sa.run_simulated_annealing()

    # Output the final duct paths
    sa.output_paths()
    return sa, terminal_locations


if __name__ == "__main__":
    app.run()
